using Domain.Validation;
using System;

namespace Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(150)]
        public string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public string StreetNumber { get; set; }

        [MaxLength(20)]
        public string ApartmentNumber { get; set; }
    }
}
