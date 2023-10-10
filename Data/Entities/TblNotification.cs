using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblNotification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? TradeId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime UpdateAt { get; set; }

        public virtual TblPost? Post { get; set; }
        public virtual TblUser User { get; set; } = null!;
    }
}
