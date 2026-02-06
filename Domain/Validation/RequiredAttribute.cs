namespace Domain.Validation
{
    public sealed class RequiredAttribute : ValidationAttribute
    {
        public RequiredAttribute(string errorMessage = "Field is required")
            : base(errorMessage) { }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            if (value is string s)
                return !string.IsNullOrWhiteSpace(s);

            return true;
        }
    }
}
