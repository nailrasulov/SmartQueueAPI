using SmartQueue.Application.DTO_s;
using SmartQueue.Application.Services.Interfaces;
using SmartQueue.Domain.Common;
using SmartQueue.Domain.Entities;
using SmartQueue.Domain.Enums;
using SmartQueue.Infrastructure.Repositories.Interfaces;

namespace SmartQueue.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<CustomerResponseDto>> CallNextCustomerAsync(CancellationToken cancellationToken = default)
        {
            var nextCustomer = await _repository.GetNextWaitingAndServeCustomerAsync(cancellationToken);
            if (nextCustomer == null)
            {
                return Result.Failure<CustomerResponseDto>("Novbede gozleyen musteri yoxdur.");
            }

            return Result.Success(MapToDto(nextCustomer, null));
        }

        public async Task<Result<CustomerResponseDto>> CompleteCustomerAsync(int id, CancellationToken cancellationToken = default)
        {
            var completedCustomer = await _repository.CompleteCustomerAsync(id, cancellationToken);

            if (completedCustomer == null )
            {
                return Result.Failure<CustomerResponseDto>("Musteri tapilmadi ve ya (Serving) xidmetde deyil");
            }

            return Result.Success(MapToDto(completedCustomer, null));
        }

        public async Task<Result<CustomerResponseDto>> CreateCustomerAsync(CreateCustomerRequestDto dto, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Result.Failure<CustomerResponseDto>("Musteri adi bos ola bilmez.");
            }

            var customer = new Customer
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                Status = CustomerStatus.Waiting
            };

            var createdCustomer = await _repository.AddAsync(customer, cancellationToken);
            var position = await _repository.GetPositionInQueueAsync(createdCustomer.Id, cancellationToken);

            return Result.Success(MapToDto(createdCustomer, position));
        }

        public async Task<Result> DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await _repository.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                return Result.Failure($"Musteri tapilmadi ve ya siline bilmedi. Id: {id}");
            }
            return Result.Success();
        }

        public async Task<Result<CustomerResponseDto>> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var customer = await _repository.GetByIdAsync(id, cancellationToken);
            if (customer == null)
            {
                return Result.Failure<CustomerResponseDto>("Musteri tapilmadi.");
            }

            int? position = customer.Status == CustomerStatus.Waiting ? await _repository.GetPositionInQueueAsync(id, cancellationToken) : null;

            return Result.Success(MapToDto(customer, position));
        }

        public async Task<Result<List<CustomerResponseDto>>> GetWaitingCustomersAsync(CancellationToken cancellationToken = default)
        {
            var customers = await _repository.GetWaitingCustomersAsync(cancellationToken);
            var dtos = new List<CustomerResponseDto>();

            for (int i = 0; i < customers.Count; i++)
            {
                dtos.Add(MapToDto(customers[i], i + 1));
            }

            return Result.Success(dtos);
        }

        private static CustomerResponseDto MapToDto(Customer entity, int? position)
        {
            return new CustomerResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                CompletedAt = entity.CompletedAt,
                Status = entity.Status.ToString(),
                QueuePosition = position
            };

        }
    }
}
