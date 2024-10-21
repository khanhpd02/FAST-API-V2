using FAST_API_V2.ViewModels;

namespace FAST_API_V2.Services
{
    public interface IUserService
    {
        //Task<LoginOutputVM> Login(LoginInputVM vm, string connect);
        Task<Dictionary<string, object>> Login(LoginInputVM vm, string connect);
        Task<Dictionary<string, object>> ChangePassword(ChangePasswordVM vm, string connect);
        Task<Dictionary<string, object>> CountRequest(CountRequestVM vm, string connect);

    }
}
