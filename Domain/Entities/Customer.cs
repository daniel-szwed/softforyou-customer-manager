using Domain.Validation;
using System;

namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]

        [Required]
        [MaxLength(50)]
        public string TaxId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        [Email]
        [MaxLength(320)]
        public string EmailAddress { get; set; }

        // Navigation 
        public Address Address { get; set; }
    }
}
