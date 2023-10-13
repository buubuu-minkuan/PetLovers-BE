using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostModel
{
    public class PostReqModel
    {
        public String? content { get; set; }
        public String? attachment { get; set; }
    }

    public class PostCreateReqModel
    {
        public string token { get; set; } = null!;
        public string content { get; set; } = null!;
        public string attachment { get; set; } = null!;
    }
}
