using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.Interfaces;

namespace market_mvc.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request)
        {
            var query = _context.Orders.AsQueryable();

            // Apply search filter (search by order ID)
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(o => o.Id.ToString().Contains(request.Search));
            }

            // Apply total amount filters
            if (request.MinTotal.HasValue)
            {
                query = query.Where(o => o.Total >= request.MinTotal.Value);
            }

            if (request.MaxTotal.HasValue)
            {
                query = query.Where(o => o.Total <= request.MaxTotal.Value);
            }

            // Apply date filters
            if (request.StartDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= request.EndDate.Value);
            }

            // Apply date range filter
            if (!string.IsNullOrEmpty(request.DateRange))
            {
                var now = DateTime.UtcNow;
                switch (request.DateRange.ToLower())
                {
                    case "today":
                        var today = now.Date;
                        query = query.Where(o => o.CreatedAt >= today && o.CreatedAt < today.AddDays(1));
                        break;
                    case "week":
                        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
                        query = query.Where(o => o.CreatedAt >= weekStart);
                        break;
                    case "month":
                        var monthStart = new DateTime(now.Year, now.Month, 1);
                        query = query.Where(o => o.CreatedAt >= monthStart);
                        break;
                    case "year":
                        var yearStart = new DateTime(now.Year, 1, 1);
                        query = query.Where(o => o.CreatedAt >= yearStart);
                        break;
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "total":
                        query = request.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(o => o.Total)
                            : query.OrderBy(o => o.Total);
                        break;
                    case "createdat":
                    case "date":
                        query = request.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(o => o.CreatedAt)
                            : query.OrderBy(o => o.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(o => o.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt);
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public Task UpdateAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
        }
    }
}

