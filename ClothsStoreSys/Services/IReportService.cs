namespace ClothsStoreSys.Services
{
    public interface IReportService
    {
        Task<decimal> GetDailySalesTotalAsync(DateTime date);
    }
}