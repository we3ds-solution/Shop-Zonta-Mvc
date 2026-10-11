using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Controllers.Api
{
    /// <summary>
    /// Read-only product API (Swagger-documented, consumed by the mobile app
    /// and third-party integrations). No admin/mutation endpoints here —
    /// those stay behind MVC + auth. Rate-limited by the global auth policy
    /// only if marked; public catalog reads are intentionally open.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db) => _db = db;

        /// <summary>
        /// List active products (paginated).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Products
                .Where(p => !p.IsDeleted && p.Status == Domain.Enums.ProductStatus.Active);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.CompareAtPrice,
                    p.ImageUrl,
                    p.ImageAlt,
                    Category = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();

            return Ok(new { data = items, total, page, pageSize });
        }

        /// <summary>
        /// Get a single product by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var p = await _db.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (p == null) return NotFound();

            return Ok(new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CompareAtPrice,
                p.ImageUrl,
                p.ImageAlt,
                p.Sku,
                p.StockQuantity,
                p.TrackQuantity,
                Category = p.Category != null ? p.Category.Name : null
            });
        }
    }
}
