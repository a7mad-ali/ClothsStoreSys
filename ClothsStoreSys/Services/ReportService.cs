using ClothsStoreSys.Data;
using System.Linq; // Add this for LINQ extension methods

namespace ClothsStoreSys.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        public ReportService(AppDbContext db) { _db = db; }

        public async Task<decimal> GetDailySalesTotalAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            // Use synchronous LINQ since Invoices is a List<Invoice>
            var total = _db.Invoices
                .Where(i => i.Date >= start && i.Date < end)
                .Sum(i => (decimal?)i.Total) ?? 0m;
            return await Task.FromResult(total);
        }
    }
}