using Data.Models.OTPVerifyModel;
using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.VerifyServices
{
    public interface IVerifyServices
    {
        public Task<ResultModel> SendVerifyEmailOTP(string token);
        public Task<ResultModel> SendVerifyResetPassword(string email);
        public Task<ResultModel> VerifyOTPCode(OTPVerifyReqModel OTPCode);
    }
}