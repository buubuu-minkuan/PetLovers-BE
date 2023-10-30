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
        public Task<ResultModel> SendMail(string token);
        public Task<ResultModel> VerifyOTPCode(string OTP, string token);
    }
}