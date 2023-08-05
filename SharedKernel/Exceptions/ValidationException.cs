namespace SharedKernel.Exceptions
{
    using FluentValidation.Results;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public ValidationException(ValidationFailure failure)
            : this()
        {
            Errors = new Dictionary<string, string[]>
            {
                [failure.PropertyName] = new[] { failure.ErrorMessage }
            };
        }

        public ValidationException(string errorPropertyName, string errorMessage)
            : base(errorMessage)
        {
            Errors = new Dictionary<string, string[]>
            {
                [errorPropertyName] = new[] { errorMessage }
            };
        }

        public ValidationException(string errorMessage)
            : base(errorMessage)
        {
            Errors = new Dictionary<string, string[]>
            {
                ["Validation Exception"] = new[] { errorMessage }
            };
        }

        public IDictionary<string, string[]> Errors { get; }

        public static void ThrowWhenNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
                throw new ValidationException(message);
        }
        public static void ThrowWhenNullOrWhitespace(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(message);
        }
        public static void ThrowWhenEmpty(Guid value, string message)
        {
            if (value == Guid.Empty)
                throw new ValidationException(message);
        }
        public static void ThrowWhenNullOrEmpty(Guid? value, string message)
        {
            if (value == null || value == Guid.Empty)
                throw new ValidationException(message);
        }
        public static void ThrowWhenNull(int? value, string message)
        {
            if (value == null)
                throw new ValidationException(message);
        }
        public static void ThrowWhenNull(object value, string message)
        {
            if (value == null)
                throw new ValidationException(message);
        }
        public static void Must(bool condition, string message)
        {
            if(!condition)
                throw new ValidationException(message);
        }
        public static void MustNot(bool condition, string message)
        {
            if (condition)
                throw new ValidationException(message);
        }
    }
}