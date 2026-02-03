using System;

namespace Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }

        public string PostCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string ApartmentNumber { get; set; }
    }
}
