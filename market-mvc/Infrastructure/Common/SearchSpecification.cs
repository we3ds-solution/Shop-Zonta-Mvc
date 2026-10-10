using System.Linq.Expressions;

namespace market_mvc.Infrastructure.Common
{
    /// <summary>
    /// Base specification for search, filter, pagination, and sorting
    /// Provides common search functionality across all entities
    /// </summary>
    public abstract class SearchSpecification<T> : BaseSpecification<T>
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = "asc";
        public bool IsPagingEnabled { get; set; } = true;

        protected SearchSpecification()
        {
            ApplyPaging();
        }

        protected SearchSpecification(Expression<Func<T, bool>> criteria) 
            : base(criteria)
        {
            ApplyPaging();
        }

        /// <summary>
        /// Apply search filter to the specification
        /// </summary>
        protected void ApplySearch(Expression<Func<T, bool>> searchExpression)
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                AddCriteria(searchExpression);
            }
        }

        /// <summary>
        /// Apply sorting to the specification
        /// </summary>
        protected void ApplyOrdering(Expression<Func<T, dynamic>> orderExpression)
        {
            if (string.IsNullOrWhiteSpace(OrderBy))
                return;

            if (OrderDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                AddOrderByDescending(orderExpression);
            }
            else
            {
                AddOrderBy(orderExpression);
            }
        }

        /// <summary>
        /// Apply pagination
        /// </summary>
        protected void ApplyPaging()
        {
            if (!IsPagingEnabled)
                return;

            var skip = (PageNumber - 1) * PageSize;
            ApplyPaging(skip, PageSize);
        }

        /// <summary>
        /// Get total items count for pagination
        /// </summary>
        public int GetSkip() => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Get page size
        /// </summary>
        public int GetPageSize() => PageSize;

        /// <summary>
        /// Validate pagination parameters
        /// </summary>
        public void ValidatePaginationParameters()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 10;

            if (PageSize > 100)
                PageSize = 100;
        }
    }

    /// <summary>
    /// Generic search request object
    /// </summary>
    public class SearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = "asc";

        public void Validate()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 10;

            if (PageSize > 100)
                PageSize = 100;

            if (string.IsNullOrWhiteSpace(OrderDirection))
                OrderDirection = "asc";
        }
    }

    /// <summary>
    /// Advanced filter options
    /// </summary>
    public class FilterOptions
    {
        public Dictionary<string, object> Filters { get; set; } = new();
        public List<string> IncludeFields { get; set; } = new();
        public List<string> ExcludeFields { get; set; } = new();

        public FilterOptions AddFilter(string key, object value)
        {
            if (Filters.ContainsKey(key))
                Filters[key] = value;
            else
                Filters.Add(key, value);

            return this;
        }

        public FilterOptions IncludeField(string field)
        {
            if (!IncludeFields.Contains(field))
                IncludeFields.Add(field);
            return this;
        }

        public FilterOptions ExcludeField(string field)
        {
            if (!ExcludeFields.Contains(field))
                ExcludeFields.Add(field);
            return this;
        }

        public object? GetFilter(string key)
        {
            return Filters.TryGetValue(key, out var value) ? value : null;
        }

        public bool HasFilter(string key) => Filters.ContainsKey(key);

        public void Clear()
        {
            Filters.Clear();
            IncludeFields.Clear();
            ExcludeFields.Clear();
        }
    }

    /// <summary>
    /// Range filter for numeric values
    /// </summary>
    public class RangeFilter
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public RangeFilter(decimal? min = null, decimal? max = null)
        {
            Min = min;
            Max = max;
        }

        public bool IsValid()
        {
            if (Min.HasValue && Max.HasValue)
                return Min <= Max;
            return Min.HasValue || Max.HasValue;
        }
    }

    /// <summary>
    /// Date range filter
    /// </summary>
    public class DateRangeFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateRangeFilter(DateTime? startDate = null, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool IsValid()
        {
            if (StartDate.HasValue && EndDate.HasValue)
                return StartDate <= EndDate;
            return StartDate.HasValue || EndDate.HasValue;
        }
    }
}
