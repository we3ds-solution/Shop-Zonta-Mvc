using market_mvc.Repositoriers; using market_mvc.Interfaces;

namespace market_mvc.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepo Products { get; }
        IOrderRepository Orders { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}

