namespace market_mvc.Infrastructure.Common
{
    /// <summary>
    /// Base application exception
    /// </summary>
    public class ApplicationException : Exception
    {
        public string Code { get; set; }

        public ApplicationException(string message, string code = "ERROR") 
            : base(message)
        {
            Code = code;
        }

        public ApplicationException(string message, Exception innerException, string code = "ERROR")
            : base(message, innerException)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Thrown when requested resource is not found
    /// </summary>
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message, string code = "NOT_FOUND")
            : base(message, code)
        {
        }

        public NotFoundException(string resource, object id)
            : base($"{resource} with ID '{id}' was not found", "NOT_FOUND")
        {
        }
    }

    /// <summary>
    /// Thrown when operation violates business rules
    /// </summary>
    public class BusinessRuleException : ApplicationException
    {
        public BusinessRuleException(string message, string code = "BUSINESS_RULE_VIOLATION")
            : base(message, code)
        {
        }
    }

    /// <summary>
    /// Thrown when data validation fails
    /// </summary>
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Failures { get; set; }

        public ValidationException(string message, string code = "VALIDATION_ERROR")
            : base(message, code)
        {
            Failures = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> failures, string code = "VALIDATION_ERROR")
            : base("One or more validation errors occurred.", code)
        {
            Failures = failures;
        }

        public ValidationException(string fieldName, string error, string code = "VALIDATION_ERROR")
            : base("One or more validation errors occurred.", code)
        {
            Failures = new Dictionary<string, string[]> { { fieldName, new[] { error } } };
        }
    }

    /// <summary>
    /// Thrown when user is not authorized to perform operation
    /// </summary>
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message = "User is not authorized", string code = "UNAUTHORIZED")
            : base(message, code)
        {
        }
    }

    /// <summary>
    /// Thrown when user access is forbidden
    /// </summary>
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message = "Access to this resource is forbidden", string code = "FORBIDDEN")
            : base(message, code)
        {
        }
    }

    /// <summary>
    /// Thrown when operation results in a conflict
    /// </summary>
    public class ConflictException : ApplicationException
    {
        public ConflictException(string message, string code = "CONFLICT")
            : base(message, code)
        {
        }
    }

    /// <summary>
    /// Thrown when a duplicate resource already exists
    /// </summary>
    public class DuplicateException : ConflictException
    {
        public DuplicateException(string resource, string field, object value)
            : base($"{resource} with {field} '{value}' already exists", "DUPLICATE_ENTRY")
        {
        }

        public DuplicateException(string message)
            : base(message, "DUPLICATE_ENTRY")
        {
        }
    }

    /// <summary>
    /// Thrown when database operation fails
    /// </summary>
    public class DataAccessException : ApplicationException
    {
        public DataAccessException(string message, string code = "DATABASE_ERROR")
            : base(message, code)
        {
        }

        public DataAccessException(string message, Exception innerException, string code = "DATABASE_ERROR")
            : base(message, innerException, code)
        {
        }
    }
}
