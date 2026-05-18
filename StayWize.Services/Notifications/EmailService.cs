using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace StayWize.Services.Notifications;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string body)
    {
        try
        {
            var smtp = _configuration.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                smtp["FromName"] ?? "StayWize",
                smtp["FromEmail"] ?? "noreply@staywize.com"));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            var host = smtp["Host"] ?? throw new InvalidOperationException("SmtpSettings:Host no está configurado.");
            var username = smtp["Username"] ?? throw new InvalidOperationException("SmtpSettings:Username no está configurado.");
            var password = smtp["Password"] ?? throw new InvalidOperationException("SmtpSettings:Password no está configurado.");

            using var client = new SmtpClient();
            await client.ConnectAsync(host, int.Parse(smtp["Port"] ?? "587"), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado a {Email} con asunto: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
        }
    }

    public async Task SendInvitationAsync(
        string toEmail, string toName, string role,
        string token, DateTime expiresAt)
    {
        // La URL base debería venir de configuración en producción
        var link = $"https://app.staywize.com/complete-registration?token={token}";

        var roleDisplay = role switch
        {
            "Owner" => "Propietario",
            "HostLocal" => "Host Local",
            "Guest" => "Huésped",
            "Admin" => "Administrador",
            _ => role
        };

        var subject = $"Invitación a StayWize — {roleDisplay}";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Fuiste invitado a unirte a <strong>StayWize</strong> como <strong>{roleDisplay}</strong>.</p>
            <p>Para completar tu cuenta, hacé clic en el siguiente enlace:</p>
            <p>
              <a href="{link}" style="
                display: inline-block;
                background-color: #2563eb;
                color: white;
                padding: 12px 24px;
                border-radius: 6px;
                text-decoration: none;
                font-weight: bold;">
                Completar registro
              </a>
            </p>
            <p style="color: #6b7280; font-size: 14px;">
              Este enlace expira el <strong>{expiresAt:dd/MM/yyyy HH:mm}</strong> UTC.
              Si no esperabas esta invitación, podés ignorar este mensaje.
            </p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    public async Task SendReservationConfirmedAsync(
        string toEmail, string toName, string propertyName,
        DateTime checkIn, DateTime checkOut)
    {
        var subject = "Tu reserva fue confirmada — StayWize";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Tu reserva en <strong>{propertyName}</strong> fue confirmada exitosamente.</p>
            <p><strong>Check-in:</strong> {checkIn:dd/MM/yyyy HH:mm}</p>
            <p><strong>Check-out:</strong> {checkOut:dd/MM/yyyy HH:mm}</p>
            <p>Pronto recibirás tu código de acceso.</p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    public async Task SendAccessCodeGeneratedAsync(
        string toEmail, string toName, string code,
        DateTime validFrom, DateTime validTo)
    {
        var subject = "Tu código de acceso — StayWize";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Tu código de acceso para la propiedad es:</p>
            <h1 style="letter-spacing: 8px; color: #2563eb;">{code}</h1>
            <p><strong>Válido desde:</strong> {validFrom:dd/MM/yyyy HH:mm}</p>
            <p><strong>Válido hasta:</strong> {validTo:dd/MM/yyyy HH:mm}</p>
            <p>Guardá este código, lo vas a necesitar para ingresar a la propiedad.</p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    public async Task SendAccessCodeRevokedAsync(string toEmail, string toName, string propertyName)
    {
        var subject = "Tu código de acceso fue revocado — StayWize";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Tu código de acceso para <strong>{propertyName}</strong> fue revocado.</p>
            <p>Si tenés alguna consulta, contactá al propietario.</p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }

    public async Task SendAccessCodeExpiredAsync(string toEmail, string toName, string propertyName)
    {
        var subject = "Tu código de acceso ha expirado — StayWize";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Tu código de acceso para <strong>{propertyName}</strong> ha expirado.</p>
            <p>Si necesitás un nuevo código, contactá al propietario.</p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }
}