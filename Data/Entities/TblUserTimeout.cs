using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblUserTimeout
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; } = null!;
        public Guid ModeratorId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpiredAt { get; set; }

        public virtual TblUser Moderator { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
    }
}
