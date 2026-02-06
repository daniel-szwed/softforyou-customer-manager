using System;

namespace Domain.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ValidationAttribute : Attribute
    {
        public string ErrorMessage { get; }

        protected ValidationAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public abstract bool IsValid(object value);
    }
}
