using FAST_API_V2.Services;
using FAST_API_V2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FAST_API_V2.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("/login-trucking.html")]
        public async Task<Dictionary<string, object>> LoginTrucking([FromBody] LoginInputVM loginInputVM)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _userService.Login(loginInputVM, connect);
        }
        [AllowAnonymous]
        [HttpPost("/change-password.html")]
        public async Task<Dictionary<string, object>> ChangePassword([FromBody] ChangePasswordVM changePasswordVM)
        {

            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _userService.ChangePassword(changePasswordVM, connect);
        }
        [AllowAnonymous]
        [HttpPost("/count-request.html")]
        public async Task<Dictionary<string, object>> CountRequest([FromBody] CountRequestVM countRequestVM)
        {

            var connect = _configuration.GetConnectionString("FAST-DEMO");
            var rp= await _userService.CountRequest(countRequestVM, connect);
            return rp;
        }
    }
}
