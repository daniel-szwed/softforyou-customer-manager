using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer[]> GetAllCustomersAsync();
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(Guid customerId);
        Task<bool> IsTaxIdAlreadyTakenAsync(string taxId, Guid excludedCustomerTaxId);
    }
}
