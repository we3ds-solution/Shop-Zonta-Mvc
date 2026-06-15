using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Infrastructure.Common;

namespace market_mvc.Features.Orders.Specifications
{
    /// <summary>
    /// Search specification for orders
    /// Encapsulates order search, filter, sort, and pagination logic
    /// </summary>
    public class OrderSearchSpecification : SearchSpecification<Order>
    {
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        public OrderSearchSpecification(
            string searchTerm = "",
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "CreatedAt",
            string orderDirection = "desc",
            decimal? minTotal = null,
            decimal? maxTotal = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null)
        {
            SearchTerm = searchTerm;
            PageNumber = pageNumber;
            PageSize = pageSize;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            MinTotal = minTotal;
            MaxTotal = maxTotal;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
            PaymentStatus = paymentStatus;

            ValidatePaginationParameters();

            // Build search filter
            BuildSearchFilter();

            // Build total filter
            BuildTotalFilter();

            // Build date range filter
            BuildDateRangeFilter();

            // Build status filters
            BuildStatusFilters();

            // Apply ordering
            ApplyOrdering(GetOrderExpression());

            // Apply pagination
            ApplyPaging();

            // Include related entities
            AddInclude(o => o.OrderItems);
        }

        private void BuildSearchFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return;

            var term = SearchTerm.Trim().ToLower();
            ApplySearch(o =>
                o.OrderNumber.ToLower().Contains(term) ||
                o.CustomerEmail.ToLower().Contains(term) ||
                o.CustomerName.FirstName.ToLower().Contains(term) ||
                o.CustomerName.LastName.ToLower().Contains(term) ||
                o.TrackingNumber.Contains(term)
            );
        }

        private void BuildTotalFilter()
        {
            if (MinTotal.HasValue || MaxTotal.HasValue)
            {
                var hasMin = MinTotal.HasValue;
                var hasMax = MaxTotal.HasValue;

                if (hasMin && hasMax)
                {
                    AddCriteria(o => o.Total >= MinTotal && o.Total <= MaxTotal);
                }
                else if (hasMin)
                {
                    AddCriteria(o => o.Total >= MinTotal);
                }
                else if (hasMax)
                {
                    AddCriteria(o => o.Total <= MaxTotal);
                }
            }
        }

        private void BuildDateRangeFilter()
        {
            if (StartDate.HasValue || EndDate.HasValue)
            {
                var hasStart = StartDate.HasValue;
                var hasEnd = EndDate.HasValue;

                if (hasStart && hasEnd)
                {
                    AddCriteria(o => o.CreatedAt >= StartDate && o.CreatedAt <= EndDate);
                }
                else if (hasStart)
                {
                    AddCriteria(o => o.CreatedAt >= StartDate);
                }
                else if (hasEnd)
                {
                    AddCriteria(o => o.CreatedAt <= EndDate);
                }
            }
        }

        private void BuildStatusFilters()
        {
            if (Status.HasValue)
            {
                AddCriteria(o => o.Status == Status.Value);
            }

            if (PaymentStatus.HasValue)
            {
                AddCriteria(o => o.PaymentStatus == PaymentStatus.Value);
            }
        }

        private Expression<Func<Order, dynamic>> GetOrderExpression()
        {
            return OrderBy?.ToLower() switch
            {
                "total" => o => o.Total,
                "date" => o => o.CreatedAt,
                "status" => o => o.Status,
                "customername" => o => o.CustomerName.FirstName,
                _ => o => o.CreatedAt
            };
        }
    }
}
