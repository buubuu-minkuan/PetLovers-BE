using Data.Models.ResultModel;
namespace Business.Services.UserServices
{
    public interface IUserServices
    {
        public Task<ResultModel> Register(string Name, string Email, string Username, string Password, string Phone);

        public Task<ResultModel> Login(string Username, string Password);

        public ResultModel ReadJWT(string jwtToken, string secretkey, string issuer);

        public Task<ResultModel> GetUser(Guid id);
    }
}