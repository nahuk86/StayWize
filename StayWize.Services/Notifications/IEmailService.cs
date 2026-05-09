namespace StayWize.Services.Notifications;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string body);
    Task SendReservationConfirmedAsync(string toEmail, string toName, string propertyName, DateTime checkIn, DateTime checkOut);
    Task SendAccessCodeGeneratedAsync(string toEmail, string toName, string code, DateTime validFrom, DateTime validTo);
    Task SendAccessCodeRevokedAsync(string toEmail, string toName, string propertyName);
    Task SendAccessCodeExpiredAsync(string toEmail, string toName, string propertyName);
}