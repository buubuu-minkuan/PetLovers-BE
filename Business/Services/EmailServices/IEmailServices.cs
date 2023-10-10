using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.EmailServices
{
    public interface IEmailServices
    {
        public Task<ResultModel> SendMail(string toEmail);
    }
}