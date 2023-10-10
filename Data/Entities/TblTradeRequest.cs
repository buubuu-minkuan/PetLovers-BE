using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblTradeRequest
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = null!;
        public bool IsProcessed { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TblPost Post { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
    }
}
