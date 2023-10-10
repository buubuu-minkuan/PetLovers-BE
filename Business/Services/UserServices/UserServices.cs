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

namespace Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepo _userRepo;

        public UserServices(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ResultModel> Register(string Name, string Email, string Username, string Password, string Phone)
        {
            ResultModel result = new();
            try
            {
                var getUserRoleId = await _userRepo.GetRoleId(Commons.USER);
                var checkUserUsername = await _userRepo.getUserByUsername(Username);
                var checkUserEmail = await _userRepo.GetUserByEmail(Email);
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
                byte[] HashPassword = UserAuthentication.CreatePasswordHash(Password);
                DateTime Date = DateTime.Now;
                TblUser UserModel = new TblUser()
                {
                    Email = Email,
                    Password = HashPassword,
                    Username = Username,
                    Name = Name,
                    Phone = Phone,
                    Status = UserStatus.ACTIVE,
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

        public async Task<ResultModel> Login(string Username, string Password)
        {
            ResultModel result = new();
            try
            {
                var User = await _userRepo.getUserByUsername(Username);
                if (User == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User khong ton tai";
                    return result;
                }
                byte[] HashPasswordInput = UserAuthentication.CreatePasswordHash(Password);
                bool isMatch = UserAuthentication.VerifyPasswordHash(HashPasswordInput, User.Password);
                if (!isMatch)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Mat khau sai";
                    return result;
                }
                string token = UserAuthentication.GenerateJWT(User);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = token;
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

        public ResultModel ReadJWT(string jwtToken, string secretkey, string issuer)
        {
            ResultModel result = new();
            try
            {
                var User = UserAuthentication.ReadJwtToken(jwtToken, secretkey, issuer, ref result);
                if (User == false)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                //result.Data = User;
                result.Message = "JWT da duoc xac thuc";
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

        public async Task<ResultModel> GetUser(Guid id)
        {
            ResultModel result = new();
            try
            {
                var User = await _userRepo.GetUserById(id);
                if (User == null)
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
    }
}
