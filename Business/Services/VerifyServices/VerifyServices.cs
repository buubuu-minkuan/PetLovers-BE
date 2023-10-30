using Data.Entities;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.OTPRepository;
using Data.Repositories.PostRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using Business.Services.SecretServices;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using static System.Net.WebRequestMethods;
using System.Net.Mail;
using Data.Repositories.UserRepo;
using Business.Ultilities.UserAuthentication;

namespace Business.Services.VerifyServices
{
    public class VerifyServices : IVerifyServices
    {
        private readonly IOTPRepo _OTPRepo;
        private readonly IUserRepo _userRepo;
        private readonly UserAuthentication _userAuthentication;

        public VerifyServices(IOTPRepo OTPRepo, IUserRepo userRepo)
        {
            _userRepo = userRepo;
            _OTPRepo = OTPRepo;
            _userAuthentication = new UserAuthentication();
        }

        private string CreateOTPCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }

        public async Task<ResultModel> SendMail(string token)
        {
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            string OTPCode = CreateOTPCode();
            DateTime expiredAt = DateTime.Now.AddMinutes(10);
            ResultModel result = new();
            try
            {
                TblOtpverify newOTP = new()
                {
                    UserId = userId,
                    OtpCode = OTPCode,
                    ExpiredAt = expiredAt,
                };
                _ = await _OTPRepo.Insert(newOTP);
                var user = await _userRepo.Get(userId);
                var toEmail = user.Email;
                string from = SecretService.GetSMTPEmail();
                string pass = SecretService.GetSMTPPass();
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(SecretService.GetSMTPEmail()));
                message.Subject = "[PETLOVERS - VERIFY YOUR ACCOUNT]";
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text =
                    "<html>" +
                    "<body>" +
                    "<h1>PetLovers<h1>" +
                    "<h3>Chào mừng bạn đã đến với Cộng đồng yêu thích chó mèo PetLovers</h3>" +
                    "<p>Để bắt đầu sử dụng tài khoản <b>" + toEmail + "</b> bạn hãy vui lòng xác thực tài khoản bằng cách nhập OTP ở bên dưới</p>" +
                    "<p>Mã của bạn là: <b>" + OTPCode + "</b></p>" +
                    "</body>" +
                    "</html>"
                };
                using MailKit.Net.Smtp.SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, pass);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
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

        public async Task<ResultModel> VerifyOTPCode(string OTP, string token)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            try
            {

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