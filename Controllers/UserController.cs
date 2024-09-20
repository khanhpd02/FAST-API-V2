using FAST_API_V2.Services;
using FAST_API_V2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FAST_API_V2.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("/login-trucking.html")]
        public async Task<Dictionary<string, object>> LoginTrucking([FromBody] LoginInputVM loginInputVM)
        {
            var connect = _configuration.GetConnectionString("FAST-DEMO");
            return await _userService.Login(loginInputVM, connect);
        }
    }
}
