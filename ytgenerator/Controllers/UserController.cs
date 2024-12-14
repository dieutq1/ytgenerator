using Microsoft.AspNetCore.Mvc;
using ytgenerator.Services;
using ytgenerator.Shared.Requests;

namespace ytgenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserServices userServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;

        [HttpPost("register")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            var response = await _userServices.CreateUserAsync(request);
            return Ok(response);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            var response = await _userServices.ChangePasswordAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var response = await _userServices.Login(request);

            if (!response.IsSucceeded)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [HttpGet("get-info")]
        public async Task<IActionResult> GetUserInfoByToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _userServices.GetUserInfoByToken(token);
            return Ok(response);
        }
    }
}
