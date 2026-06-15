namespace market_mvc.Services
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile imageFile, string folder = "products");
        Task<bool> DeleteImageAsync(string imageUrl);
        bool IsValidImageFile(IFormFile file);
        string GetImagePath(string fileName, string folder = "products");
    }
}

