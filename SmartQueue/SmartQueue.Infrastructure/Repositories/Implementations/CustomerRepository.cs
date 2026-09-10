using Microsoft.EntityFrameworkCore;
using SmartQueue.Domain.Entities;
using SmartQueue.Domain.Enums;
using SmartQueue.Infrastructure.Context;
using SmartQueue.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Infrastructure.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return customer;
        }

        public async Task<Customer?> CompleteCustomerAsync(int id, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (customer == null || (customer.Status != CustomerStatus.Serving))
            {
                return null;
            }    

            customer.Status = CustomerStatus.Completed;
            customer.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return customer;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (customer == null)
            {
                return false;
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return customer;
        }

        public async Task<Customer?> GetNextWaitingAndServeCustomerAsync(CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var nextCustomer = await _context.Customers
                .FromSql($"Select Top 1 * from Customers with (UPDLOCK, READPAST) where Status = {CustomerStatus.Waiting.ToString()} Order By CreatedAt ASC")
                .FirstOrDefaultAsync(cancellationToken);

            if (nextCustomer == null)
            {
                return null;
            }

            nextCustomer.Status = CustomerStatus.Serving;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return nextCustomer;
        }

        public async Task<int> GetPositionInQueueAsync(int id, CancellationToken cancellationToken = default)
        {
            var targetCustomer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (targetCustomer == null || targetCustomer.Status != CustomerStatus.Waiting)
            {
                return -1;
            }

            var position = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Status == CustomerStatus.Waiting && c.CreatedAt < targetCustomer.CreatedAt)
                .CountAsync(cancellationToken);

            return position + 1;
        }

        public Task<List<Customer>> GetWaitingCustomersAsync(CancellationToken cancellationToken = default)
        {
            var customers = _context.Customers.AsNoTracking()
                                        .Where(c => c.Status == CustomerStatus.Waiting)
                                        .OrderBy(c => c.CreatedAt)
                                        .ToListAsync(cancellationToken);
            return customers;
        }
    }
}
