using market_mvc.Repositoriers; using market_mvc.Interfaces;

namespace market_mvc.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IProductRepo? _products;
        private IOrderRepository? _orders;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepo Products => _products ??= new ProductRepo(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

