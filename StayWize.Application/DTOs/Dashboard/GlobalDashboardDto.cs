namespace StayWize.Application.DTOs.Dashboard;

public class GlobalDashboardDto
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalProperties { get; set; }
    public int TotalClients { get; set; }
    public int TotalReservations { get; set; }
    public int TotalEntries { get; set; }
    public int TotalExits { get; set; }
    public int SuccessfulAccesses { get; set; }
    public int FailedAccesses { get; set; }
}