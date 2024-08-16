using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/externalapi")]
    [ApiController]
    public class ExternalApiController : ControllerBase
    {
        private readonly ExternalApiService _externalApiService;

        public ExternalApiController(ExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        [HttpPost]
        public async Task<IActionResult> CallExternalApi([FromBody] BotDataRequest model)
        {
            try
            {
                var apiResponse = await _externalApiService.CallApi(model);

                if (apiResponse != null)
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
