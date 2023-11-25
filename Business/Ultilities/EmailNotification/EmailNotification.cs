using Business.Services.SecretServices;
using Data.Entities;
using Data.Models.ResultModel;
using Data.Repositories.HashtagRepo;
using Data.Repositories.PetPostTradeRepo;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.PostStoredRepo;
using Data.Repositories.PostTradeRequestRepo;
using Data.Repositories.ReportRepo;
using Data.Repositories.UserRepo;
using Data.Repositories.UserRewardRepo;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Ultilities.EmailNotification
{
    public class EmailNotification
    {
        public EmailNotification()
        {
        }
        public async Task<bool> SendRefusePostNotification(string email, string reason, string content)
        {
            try
            {
                var toEmail = email;
                string from = SecretService.GetSMTPEmail();
                string pass = SecretService.GetSMTPPass();
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(SecretService.GetSMTPEmail()));
                message.Subject = "[PETLOVERS - NOTIFICATION]";
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text =
                    "<html>" +
                    "<body>" +
                    "<h1>PetLovers<h1>" +
                    "<h3>"+ content +"</p>" +
                    "<p>Lý do: <b>" + reason + "</b></p>" +
                    "</body>" +
                    "</html>"
                };
                using MailKit.Net.Smtp.SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, pass);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
