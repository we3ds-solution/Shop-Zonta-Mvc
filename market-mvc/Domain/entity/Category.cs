using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;
using market_mvc.Domain.Enums;

namespace market_mvc.Domain.entity
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public CategoryType Type { get; set; }
        
        [Url]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(200)]
        public string? ImageAlt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int SortOrder { get; set; } = 0;
        
        [StringLength(200)]
        public string? MetaTitle { get; set; }
        
        [StringLength(500)]
        public string? MetaDescription { get; set; }
        
        [StringLength(200)]
        public string? Slug { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Computed properties
        public int ProductCount => Products?.Count(p => !p.IsDeleted) ?? 0;
        public int ActiveProductCount => Products?.Count(p => !p.IsDeleted && p.Status == ProductStatus.Active) ?? 0;
        
        public string StatusBadgeClass => IsActive switch
        {
            true when IsDeleted => "bg-danger",
            true => "bg-success",
            false => "bg-secondary"
        };
    }
}

