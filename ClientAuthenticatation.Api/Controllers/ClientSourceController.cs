using ClientAuthentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAuthenticatation.Api.Controllers
{
    /// <summary>
    /// Thằng này có nhiệm vụ kết nối với database, db này chứa các công tin về client. Mỗi khi validator đc gọi thì sẽ kết nối tới db để xem có hợp lệ không
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSourceController : ControllerBase
    {
        private readonly IClientSourceAuthenticationHandler _handler;
        public ClientSourceController(IClientSourceAuthenticationHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (_handler.Validate(id))
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}
