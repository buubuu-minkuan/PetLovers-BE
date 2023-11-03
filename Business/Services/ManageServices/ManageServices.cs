using Business.Ultilities.UserAuthentication;
using Data.Enums;
using Data.Models.ResultModel;
using Data.Repositories.UserRepo;
using System.Security.Claims;

namespace Business.Services.ManageServices
{
    public class ManageServices : IManageServices
    {
        private readonly UserAuthentication _userAuthentication;
        private readonly IUserRepo _userRepo;

        public ManageServices(IUserRepo userRepo)
        {
            _userAuthentication = new UserAuthentication();
            _userRepo = userRepo;
        }

        public async Task<ResultModel> BanUser(List<Guid> userId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid modId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                foreach (var id in userId)
                {
                    var user = await _userRepo.Get(id);
                    user.Status = UserStatus.DEACTIVE;
                    user.UpdateAt = now;
                    _ = await _userRepo.Update(user);
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}