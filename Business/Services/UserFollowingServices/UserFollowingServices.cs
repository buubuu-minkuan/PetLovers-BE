using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.UserFollowingRepo;
using Data.Repositories.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.UserFollowingServices
{
    public class UserFollowingServices : IUserFollowingServices
    {
        private readonly IUserFollowingRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly UserAuthentication _userAuthentication;
        public UserFollowingServices(IUserFollowingRepo repo, IUserRepo userRepo)
        {
            _userAuthentication = new UserAuthentication();
            _userRepo = userRepo;
            _repo = repo;
        }
        public async Task<ResultModel> FollowUser(UserFollowingModel userFollowing)
        {
            Guid userId = new Guid(_userAuthentication.decodeToken(userFollowing.token, "userid"));
            var userFollow = await _userRepo.GetUserById(userFollowing.userId);
            ResultModel result = new();
            try
            {
                if(userFollow == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "User Follow is not exist!";
                    return result;
                }
                TblUserFollowing follow = new()
                {
                    UserId = userId,
                    FollowerId = userFollow.Id,
                    Status = Status.ACTIVE
                };
                _ = _repo.Insert(follow);
                result.IsSuccess = false;
                result.Code = 200;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        /*public async Task<ResultModel> IsFollowing(Guid userId, Guid followerId)
        {
            return await _repo.IsFollowing(userId, followerId);
        }
        public async Task<ResultModel> UnfollowUser(Guid userId, Guid followerId)
        {
            await _repo.UnfollowUser(userId, followerId);
        }*/
    }
}
