using ClothsStoreSys.Models;

namespace ClothsStoreSys.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice, List<InvoiceItem> items);
        Task<Invoice> GetByIdAsync(int id);
        Task<List<Invoice>> GetAllAsync();
    }
}