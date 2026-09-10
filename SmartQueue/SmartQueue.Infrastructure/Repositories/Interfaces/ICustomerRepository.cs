using SmartQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Infrastructure.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);

        Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Customer>> GetWaitingCustomersAsync(CancellationToken cancellationToken = default);

        Task<int> GetPositionInQueueAsync(int id, CancellationToken cancellationToken = default);

        Task<Customer?> GetNextWaitingAndServeCustomerAsync(CancellationToken cancellationToken = default);

        Task<bool> CompleteCustomerAsync(int id, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);    
    }
}
