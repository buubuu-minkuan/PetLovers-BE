using Business.Ultilities.EmailNotification;
using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.UserFollowingRepo;
using Data.Repositories.UserRepo;
using Newtonsoft.Json.Linq;
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
        private readonly EmailNotification _emailNotification;
        public UserFollowingServices(IUserFollowingRepo repo, IUserRepo userRepo)
        {
            _userAuthentication = new UserAuthentication();
            _userRepo = userRepo;
            _repo = repo;
            _emailNotification = new EmailNotification();
        }
        public async Task<ResultModel> FollowUser(UserFollowingModel userFollowing)
        {
            Guid userId = new Guid(_userAuthentication.decodeToken(userFollowing.token, "userid"));
            Guid modId = new Guid(_userAuthentication.decodeToken(userFollowing.token, "userid"));
            var userFollowed = await _repo.IsFollowing(userId, userFollowing.userId);
            var userFollow = await _userRepo.GetUserById(userFollowing.userId);
            var getuser = await _userRepo.Get(userFollowing.userId);
            var getmod = await _userRepo.Get(modId);
            ResultModel result = new();
            try
            {
                if (userFollow == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "User Follow is not exist!";
                    return result;
                }
                else if (userFollowed)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You have followed this user!";
                    return result;
                }  
                TblUserFollowing follow = new()
                {
                    UserId = userId,
                    FollowerId = userFollow.Id,
                    Status = Status.ACTIVE
                };
                _ = await _repo.Insert(follow);
                bool check = await _emailNotification.SendNotification(getuser.Email, "Bạn có một người theo dõi mới trên PetLovers: <B>" + getmod.Name + "</B>");
                if (check)
                {
                    result.Message = "Send Email Successfully!";
                }
                else
                {
                    result.Message = "Cann't Send Email!";
                }
                result.IsSuccess = true;
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
        public async Task<ResultModel> UnFollowUser(UserFollowingModel userFollowing)
        {
            Guid userId = new Guid(_userAuthentication.decodeToken(userFollowing.token, "userid"));
            ResultModel result = new();
            try
            {
                var user = await _userRepo.GetUserById(userFollowing.userId);
                var userFollow = await _repo.IsFollowing(userId, userFollowing.userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "User Follow is not exist!";
                    return result;
                } else if (!userFollow)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You did not follow this user yet!";
                    return result;
                }
                var GetUserFollowing = await _repo.GetUserFollow(userId, userFollowing.userId);
                GetUserFollowing.Status = Status.DEACTIVE;
                _ = _repo.Update(GetUserFollowing);
                result.IsSuccess = true;
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
        public async Task<ResultModel> GetFollowers(Guid userId)
        {
            ResultModel result = new();
            try
            {
                List<TblUserFollowing> listFollower = await _repo.GetFollowers(userId);
                List<UserFollowResModel> listFollowerRes = new();
                foreach(var user in listFollower)
                {
                    var getUser = await _userRepo.GetUserById(user.UserId);
                    UserFollowResModel resUser = new()
                    {
                        Id = getUser.Id,
                        Name = getUser.Name,
                        Image = getUser.Image
                    };
                    listFollowerRes.Add(resUser);
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listFollowerRes;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetFollowings(Guid userId)
        {
            ResultModel result = new();
            try
            {
                List<TblUserFollowing> listFollowing = await _repo.GetFollowing(userId);
                List<UserFollowResModel> listFollowingRes = new();
                foreach (var user in listFollowing)
                {
                    var getUser = await _userRepo.GetUserById(user.FollowerId);
                    UserFollowResModel resUser = new()
                    {
                        Id = getUser.Id,
                        Name = getUser.Name,
                        Image = getUser.Image
                    };
                    listFollowingRes.Add(resUser);
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listFollowingRes;
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
