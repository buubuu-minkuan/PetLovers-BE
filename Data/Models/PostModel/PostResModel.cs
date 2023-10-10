using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostModel
{
    public class PostResModel
    {
        public Guid id { get; set; }
        public string content { get; set; }
        public string attachment { get; set; }
        public DateTime createAt { get; set; }

    }
}
