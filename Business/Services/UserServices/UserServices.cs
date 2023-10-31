using Data.Entities;
using Data.Models.ResultModel;
using Data.Repositories.UserRepo;
using Business.Ultilities.UserAuthentication;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.UserModel;
using Newtonsoft.Json.Linq;
using Data.Repositories.UserFollowingRepo;
using Data.Repositories.PostRepo;

namespace Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepo _userRepo;
        private readonly IUserFollowingRepo _userFollowingRepo;
        private readonly IPostRepo _postRepo;
        private readonly UserAuthentication _userAuthentication;

        public UserServices(IUserRepo userRepo, IUserFollowingRepo userFollowingRepo, IPostRepo postRepo)
        {
            _postRepo = postRepo;
            _userFollowingRepo = userFollowingRepo;
            _userAuthentication = new UserAuthentication();
            _userRepo = userRepo;
        }

        public async Task<ResultModel> Register(UserResgisterModel User)
        {
            ResultModel result = new();
            try
            {
                var getUserRoleId = await _userRepo.GetRoleId(Commons.USER);
                var checkUserUsername = await _userRepo.getUserByUsername(User.Username);
                var checkUserEmail = await _userRepo.GetUserByEmail(User.Email);
                if (checkUserEmail != null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Trung Email";
                    return result;
                } else if(checkUserUsername != null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Trung username";
                }
                if (checkUserUsername != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Trung Username";
                    return result;
                }
                byte[] HashPassword = UserAuthentication.CreatePasswordHash(User.Password);
                DateTime Date = DateTime.Now;
                TblUser UserModel = new TblUser()
                {
                    Email = User.Email,
                    Password = HashPassword,
                    Username = User.Username,
                    Name = User.Name,
                    Phone = User.Phone,
                    Status = UserStatus.VERIFYING,
                    RoleId = getUserRoleId,
                    CreateAt = Date,
                };
                _ = await _userRepo.Insert(UserModel);

                result.IsSuccess = true;
                result.Code = 200;
                return result;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> Login(UserLoginReqModel User)
        {
            ResultModel result = new();
            try
            {
                var getUser = await _userRepo.getUserByUsername(User.Username);
                if (getUser == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User khong ton tai";
                    return result;
                }
                byte[] HashPasswordInput = UserAuthentication.CreatePasswordHash(User.Password);
                bool isMatch = UserAuthentication.VerifyPasswordHash(HashPasswordInput, getUser.Password);
                if (!isMatch)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Mat khau sai";
                    return result;
                }
                var roleName = await _userRepo.GetRoleName(getUser.RoleId);
                var role = new RoleModel()
                {
                    Id = getUser.RoleId,
                    Name = roleName
                };
                UserModel userModel = new UserModel()
                {
                    Id = getUser.Id,
                    Name = getUser.Name,
                    Email = getUser.Email,
                    Phone = getUser.Phone,
                    CreateAt = getUser.CreateAt,
                    Image = getUser.Image,
                    Role = role,
                    Status = getUser.Status,
                    Username = getUser.Username
                };
                string token = UserAuthentication.GenerateJWT(userModel);
                UserLoginResModel userLoginResModel = new UserLoginResModel();
                userLoginResModel.UserModel = userModel;
                userLoginResModel.token = token;
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = userLoginResModel;
                return result;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetUser(Guid id, string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var User = await _userRepo.GetUserById(id);
                var Followings = await _userFollowingRepo.GetFollowing(id);
                var Followers = await _userFollowingRepo.GetFollowers(id);
                var Posts = await _postRepo.GetPostsFromUser(id);
                if (id.Equals(userId))
                {
                    bool isVerify = false;
                    if(User.Status.Equals(UserStatus.VERIFYING))
                    {
                        isVerify = true;
                    }
                    UserPageModel userPageModel = new()
                    {
                        Id = id,
                        Name = User.Name,
                        Image = User.Image,
                        Follower = Followers.Count,
                        Following = Followings.Count,
                        IsVerify = isVerify,
                        posts = Posts,
                        Role = User.Role,
                        Username = User.Username,
                    };
                    if (User == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "User not found";
                        return result;
                    }
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = userPageModel;
                } else
                {
                    bool isFollowed = await _userFollowingRepo.IsFollowing(userId, id);
                    OtherUserPageModel userPageModel = new()
                    {
                        Id = id,
                        Name = User.Name,
                        Image = User.Image,
                        Follower = Followers.Count,
                        Following = Followings.Count,
                        IsFollowed = isFollowed,
                        posts = Posts,
                        Role = User.Role,
                        Username = User.Username,
                    };
                    if (User == null)
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "User not found";
                        return result;
                    }
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = userPageModel;
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

        public async Task<ResultModel> UpdateUser(UserUpdateReqModel model, string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var User = await _userRepo.Get(userId);
                User.Phone = model.Phone;
                User.Email = model.Email;
                User.Name = model.Name;
                if(model.Image != null)
                {
                    User.Image = model.Image;
                }
                var check = await _userRepo.Update(User);
                if (!check)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found";
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = User;
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> ChangePassword(UserChangePasswordModel model, string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var User = await _userRepo.Get(userId);
                if (User == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found";
                    return result;
                }
                byte[] hashOldPassword = UserAuthentication.CreatePasswordHash(model.oldPassword);
                bool isMatch = UserAuthentication.VerifyPasswordHash(hashOldPassword, User.Password);
                if (!isMatch)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Old password is wrong";
                    return result;
                }
                byte[] hashNewPassword = UserAuthentication.CreatePasswordHash(model.newPassword);
                User.Password = hashNewPassword;
                _ = await _userRepo.Update(User);
                result.IsSuccess = true;
                result.Code = 200;
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRoleName(Guid roleId)
        {
            ResultModel result = new();
            try
            {
                var role = await _userRepo.GetRoleName(roleId);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = role;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> ResetPassword(UserResetPasswordModel resetReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            try
            {
                var getUser = await _userRepo.Get(resetReq.UserId);
                if(getUser == null) { 
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User is not exist";
                    return result;
                }
                byte[] hashNewPassword = UserAuthentication.CreatePasswordHash(resetReq.NewPassword);
                getUser.Password = hashNewPassword;
                _ = await _userRepo.Update(getUser);
                result.IsSuccess = true;
                result.Code = 200;
                return result;
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
