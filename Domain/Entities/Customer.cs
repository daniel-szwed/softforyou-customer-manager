using System;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string TaxId { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        // Navigation (optional, Dapper won’t auto-load)
        public Address Address { get; set; }
    }
}
