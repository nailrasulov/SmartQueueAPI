using API.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SmartQueue.Application.DTO_s;
using SmartQueue.Application.Services.Implementations;
using SmartQueue.Application.Services.Interfaces;

namespace API.Controllers
{
    [Route("api/queue")]
    [ApiController]
    public class QueueController : BaseApiController
    {
        private readonly ICustomerService _service;

        public QueueController(ICustomerService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddCustomer([FromBody] CreateCustomerRequestDto dto, CancellationToken cancellationToken)
        {
         
            var result = await _service.CreateCustomerAsync(dto, cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("all-waiting")]
        public async Task<IActionResult> GetWaitingCustomers(CancellationToken cancellationToken)
        {
            var result = await _service.GetWaitingCustomersAsync(cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("get-by-id/{id}")] 
        public async Task<IActionResult> GetCustomerById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetCustomerByIdAsync(id, cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("call-next")]
        public async Task<IActionResult> CallNextCustomer(CancellationToken cancellationToken)
        {
            var result = await _service.CallNextCustomerAsync(cancellationToken);
            
            return HandleResult(result);
        }

        [HttpPost("complete/{id}")]
        public async Task<IActionResult> CompleteCustomer(int id, CancellationToken cancellationToken)
        {
            var result = await _service.CompleteCustomerAsync(id, cancellationToken);

            return HandleResult(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteCustomerAsync(id, cancellationToken);
            
            return HandleResult(result);
        }
    }
}
