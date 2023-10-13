using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enums
{
    public class PostingType
    {
        public static readonly string POSTING = "Posting";
        public static readonly string TRADING = "Trading";
    }

    public class ReactionType
    {
        public static readonly string COMMENT = "Comment";
        public static readonly string FEELING = "Feeling";
    }

    public class FeelingType
    {
        public static readonly string LIKE = "Like";
        public static readonly string HEART = "Heart";
        public static readonly string HAHA = "Haha";
        public static readonly string SAD = "Sad";
        public static readonly string WOW = "Wow";
        public static readonly string ANGRY = "Angry";
    }
}
