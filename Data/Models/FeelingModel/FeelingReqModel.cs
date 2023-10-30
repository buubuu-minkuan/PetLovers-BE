using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.FeelingModel
{
    public class FeelingReqModel
    {
        public string token {  get; set; }
        public Guid postId { get; set; }
    }
    public class FeelingCreateReqModel
    {
        public string token { get; set; }
        public Guid postId { get; set; }
    }
}
