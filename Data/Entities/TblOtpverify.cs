using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblOtpverify
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OtpCode { get; set; } = null!;
        public bool IsUsed { get; set; }
        public DateTime ExpiredAt { get; set; }

        public virtual TblUser User { get; set; } = null!;
    }
}
