namespace market_mvc.Infrastructure.Services
{
    /// <summary>
    /// Centralized search service for handling search, filter, pagination, and sorting
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Validates search parameters
        /// </summary>
        void ValidateSearchParameters(int pageNumber, int pageSize, string orderBy);

        /// <summary>
        /// Calculates skip count for pagination
        /// </summary>
        int CalculateSkip(int pageNumber, int pageSize);

        /// <summary>
        /// Builds sort expression from field name and direction
        /// </summary>
        string BuildSortExpression(string fieldName, string direction);

        /// <summary>
        /// Normalizes search term
        /// </summary>
        string NormalizeSearchTerm(string searchTerm);

        /// <summary>
        /// Builds search filter expression from multiple fields
        /// </summary>
        string BuildSearchFilter(string searchTerm, params string[] fields);

        /// <summary>
        /// Splits search term into keywords
        /// </summary>
        List<string> GetSearchKeywords(string searchTerm);
    }

    /// <summary>
    /// Default implementation of search service
    /// </summary>
    public class SearchService : ISearchService
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 10;
        private const int DefaultPageNumber = 1;

        public void ValidateSearchParameters(int pageNumber, int pageSize, string orderBy)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            if (pageSize > MaxPageSize)
                throw new ArgumentException($"Page size cannot exceed {MaxPageSize}", nameof(pageSize));
        }

        public int CalculateSkip(int pageNumber, int pageSize)
        {
            ValidateSearchParameters(pageNumber, pageSize, "");
            return (pageNumber - 1) * pageSize;
        }

        public string BuildSortExpression(string fieldName, string direction)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            var dir = direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true ? "desc" : "asc";
            return $"{fieldName} {dir}";
        }

        public string NormalizeSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return string.Empty;

            return searchTerm
                .Trim()
                .ToLower()
                .Replace("  ", " "); // Remove multiple spaces
        }

        public string BuildSearchFilter(string searchTerm, params string[] fields)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || fields.Length == 0)
                return string.Empty;

            var normalized = NormalizeSearchTerm(searchTerm);
            var filters = fields.Select(f => $"{f} LIKE '%{normalized}%'");
            return $"({string.Join(" OR ", filters)})";
        }

        public List<string> GetSearchKeywords(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<string>();

            return searchTerm
                .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim().ToLower())
                .Distinct()
                .ToList();
        }
    }
}
