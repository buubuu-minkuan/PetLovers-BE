﻿using Data.Entities;
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

namespace Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepo _userRepo;
        private readonly UserAuthentication _userAuthentication;

        public UserServices(IUserRepo userRepo)
        {
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

        public async Task<ResultModel> Login(UserLoginReqModel User)
        {
            ResultModel result = new();
            try
            {
                TblUser getUser = await _userRepo.getUserByUsername(User.Username);
                if (User == null)
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
                string token = UserAuthentication.GenerateJWT(getUser);
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

        public async Task<ResultModel> UpdateUser(UserUpdateReqModel model, string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var User = await _userRepo.Get(userId);
                User.Username = model.Username;
                User.Name = model.Name;
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
    }
}
