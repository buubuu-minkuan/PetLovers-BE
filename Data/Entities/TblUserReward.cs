using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblUserReward
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RewardId { get; set; }
        public string Status { get; set; } = null!;
        public long CreateAt { get; set; }

        public virtual TblReward Reward { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
    }
}
