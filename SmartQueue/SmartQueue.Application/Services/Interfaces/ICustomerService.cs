using SmartQueue.Application.DTO_s;
using SmartQueue.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerRequestDto dto, CancellationToken cancellationToken = default);
        Task<Result<List<CustomerResponseDto>>> GetWaitingCustomersAsync(CancellationToken cancellationToken = default);
        Task<Result<CustomerResponseDto>> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<CustomerResponseDto>> CallNextCustomerAsync(CancellationToken cancellationToken = default);
        Task<Result> DeleteCustomerAsync(int id, CancellationToken cancellationToken = default);    
    }
}
