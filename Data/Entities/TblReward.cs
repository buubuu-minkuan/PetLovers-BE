using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblReward
    {
        public TblReward()
        {
            TblUserRewards = new HashSet<TblUserReward>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int? TotalPost { get; set; }
        public int? TotalComment { get; set; }

        public virtual ICollection<TblUserReward> TblUserRewards { get; set; }
    }
}
