namespace Domain.Validation
{
    public sealed class MaxLengthAttribute : ValidationAttribute
    {
        private readonly int maxLength;

        public MaxLengthAttribute(int maxLength, string errorMessage = null)
            : base(errorMessage ?? $"Maximum length is {maxLength}")
        {
            this.maxLength = maxLength;
        }

        public override bool IsValid(object value)
        {
            return (value?.ToString().Length ?? 0) <= maxLength;
        }
    }
}
