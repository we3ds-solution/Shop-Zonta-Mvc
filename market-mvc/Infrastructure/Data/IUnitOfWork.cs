using market_mvc.Infrastructure.Repositories;
using market_mvc.Domain.Interfaces;

namespace market_mvc.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepo Products { get; }
        IOrderRepository Orders { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}

