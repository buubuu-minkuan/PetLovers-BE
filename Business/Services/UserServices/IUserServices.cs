using Data.Models.ResultModel;
using Data.Models.UserModel;

namespace Business.Services.UserServices
{
    public interface IUserServices
    {
        public Task<ResultModel> Register(UserResgisterModel User);

        public Task<ResultModel> Login(UserLoginReqModel User);

        public Task<ResultModel> GetUser(Guid id, string token);

        public Task<ResultModel> UpdateUser(UserUpdateReqModel model, string token);

        public Task<ResultModel> ChangePassword(UserChangePasswordModel model, string token);
        
        public Task<ResultModel> GetRoleName(Guid roleId);
    }
}