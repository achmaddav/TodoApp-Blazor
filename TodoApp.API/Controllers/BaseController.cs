using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Models;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            return result.StatusCode switch
            {
                200 => Ok(result),
                201 => StatusCode(201, result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        protected IActionResult HandleResult(Result result)
        {
            return result.StatusCode switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}
