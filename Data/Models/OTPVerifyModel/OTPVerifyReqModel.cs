using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.OTPVerifyModel
{
    public class OTPVerifyReqModel
    {
        public string OTP { get; set; }
        public Guid userId { get; set; }
    }
}
