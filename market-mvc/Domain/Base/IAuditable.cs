namespace market_mvc.Domain.Base
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        string? CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
    }
}

