using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartQueue.Domain.Common;

namespace API.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if(result.IsSuccess)
            {
                return Ok(BaseResponse<T>.Ok(result.Data!));
            }
            
            if(result.Error !=null && result.Error.Contains("tapilmadi"))
            {
                return NotFound(BaseResponse<T>.Fail(result.Error));
            }

            return BadRequest(BaseResponse<T>.Fail(result.Error ?? "Xeta bas verdi"));
            
        }

        protected IActionResult HandleResult(Result result)
        {
            if(result.IsSuccess)
            {
                return Ok(BaseResponse.Ok());
            }

            if(result.Error != null && result.Error.Contains("tapilmadi"))
            {
                return NotFound(BaseResponse.Fail(result.Error));
            }

            return BadRequest(BaseResponse.Fail(result.Error ?? "Xeta bas verdi"));
        }
    }
}
