namespace StayWize.Application.DTOs.Dashboard;

public class PropertyDashboardDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalReservations { get; set; }
    public int ActiveReservations { get; set; }
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public int SuccessfulAccesses { get; set; }
    public int FailedAccesses { get; set; }
    public int ActiveAccessCodes { get; set; }
    public int ExpiredAccessCodes { get; set; }
    public int RevokedAccessCodes { get; set; }
}