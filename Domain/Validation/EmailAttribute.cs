using System.Text.RegularExpressions;

namespace Domain.Validation
{
    public sealed class EmailAttribute : ValidationAttribute
    {
        private static readonly Regex regex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public EmailAttribute(string errorMessage = "Invalid email address")
            : base(errorMessage) { }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            return regex.IsMatch(value.ToString());
        }
    }
}
