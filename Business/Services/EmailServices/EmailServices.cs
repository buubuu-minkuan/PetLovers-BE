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

namespace Business.Services.EmailServices
{
    public class EmailServices : IEmailServices
    {
        private readonly IOTPRepo _OTPRepo;

        public EmailServices(IOTPRepo OTPRepo)
        {
            _OTPRepo = OTPRepo;
        }

        private string CreateOTPCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }

        public async Task<ResultModel> SendMail(string toEmail)
        {
            string OTPCode = CreateOTPCode();
            DateTime expiredAt = DateTime.Now.AddMinutes(10);
            ResultModel result = new();
            try
            {
                TblOtpverify newOTP = new()
                {
                    Email = toEmail,
                    OtpCode = OTPCode,
                    ExpiredAt = expiredAt,
                };
                _ = await _OTPRepo.Insert(newOTP);
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