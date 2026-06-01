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
        var frontendUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
        var link = $"{frontendUrl}/complete-registration?token={token}";

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
    public async Task SendNewRegistrationRequestAsync(
    string firstName, string lastName,
    string email, string documentNumber, string phone)
    {
        var notificationEmail = _configuration["AppSettings:NotificationEmail"] ?? "admin@staywize.com";
        var subject = "Nueva solicitud de alta de cliente — StayWize";
        var body = $"""
            <h2>Nueva solicitud de alta</h2>
            <p>Se recibió una nueva solicitud de registro con los siguientes datos:</p>
            <table style="border-collapse: collapse; width: 100%;">
            <tr>
                <td style="padding: 8px; border: 1px solid #e5e7eb;"><strong>Nombre</strong></td>
                <td style="padding: 8px; border: 1px solid #e5e7eb;">{firstName} {lastName}</td>
            </tr>
            <tr>
                <td style="padding: 8px; border: 1px solid #e5e7eb;"><strong>Email</strong></td>
                <td style="padding: 8px; border: 1px solid #e5e7eb;">{email}</td>
            </tr>
            <tr>
                <td style="padding: 8px; border: 1px solid #e5e7eb;"><strong>Documento</strong></td>
                <td style="padding: 8px; border: 1px solid #e5e7eb;">{documentNumber}</td>
            </tr>
            <tr>
                <td style="padding: 8px; border: 1px solid #e5e7eb;"><strong>Teléfono</strong></td>
                <td style="padding: 8px; border: 1px solid #e5e7eb;">{phone}</td>
            </tr>
            </table>
            <br/>
            <p>Ingresá al sistema para aprobar o rechazar la solicitud.</p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(notificationEmail, "Administrador StayWize", subject, body);
    }

    public async Task SendPasswordResetAsync(string toEmail, string toName, string encodedToken)
    {
        var frontendUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
        var link = $"{frontendUrl}/reset-password?token={encodedToken}&email={Uri.EscapeDataString(toEmail)}";

        var subject = "Recuperación de contraseña — StayWize";
        var body = $"""
            <h2>¡Hola {toName}!</h2>
            <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta.</p>
            <p>Hacé clic en el siguiente enlace para crear una nueva contraseña:</p>
            <p>
            <a href="{link}" style="
                display: inline-block;
                background-color: #2563eb;
                color: white;
                padding: 12px 24px;
                border-radius: 6px;
                text-decoration: none;
                font-weight: bold;">
                Restablecer contraseña
            </a>
            </p>
            <p style="color: #6b7280; font-size: 14px;">
            Este enlace expira en 24 horas.
            Si no solicitaste este cambio, podés ignorar este mensaje.
            </p>
            <br/>
            <p>El equipo de StayWize</p>
            """;

        await SendAsync(toEmail, toName, subject, body);
    }
}