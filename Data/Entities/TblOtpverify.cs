using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblOtpverify
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
    }
}
