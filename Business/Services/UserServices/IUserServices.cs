using Data.Models.ResultModel;
using Data.Models.UserModel;

namespace Business.Services.UserServices
{
    public interface IUserServices
    {
        public Task<ResultModel> Register(UserResgisterModel User);

        public Task<ResultModel> Login(UserLoginReqModel User);

        public ResultModel ReadJWT(string jwtToken);

        public Task<ResultModel> GetUser(Guid id);
    }
}