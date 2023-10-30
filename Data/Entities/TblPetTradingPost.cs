using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPetTradingPost
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public decimal Weight { get; set; }
        public string Color { get; set; } = null!;
        public string Status { get; set; } = null!;

        public virtual TblPost Post { get; set; } = null!;
    }
}
