using market_mvc.Domain.ViewModels.Product;

namespace market_mvc.Domain.ViewModels.Home
{
    public class HomeVM
    {
        public List<ProductCardVM> FeaturedProducts { get; set; } = new();
        public List<ProductCardVM> LatestProducts { get; set; } = new();
        public List<ProductCardVM> OnSaleProducts { get; set; } = new();
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
    }
}

