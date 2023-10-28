namespace Data.Enums
{
    public class Status
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string DEACTIVE = "Deactive";
    }

    public class UserStatus
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string DEACTIVE = "Deactive";
    }

    public class PostingStatus
    {
        public static readonly string PENDING = "Pending";
        public static readonly string APPROVED = "Approved";
        public static readonly string REFUSED = "Refused";
    }

    public class TradingStatus
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string INPROGRESS = "In Progress";
        public static readonly string DONE = "Done";
    }

    public class ReportingStatus
    {
        public static readonly string INPROGRESS = "In progress";
        public static readonly string COMPLETE = "Complete";
    }

    public class TradeRequestStatus
    {
        public static readonly string PENDING = "Pending";
        public static readonly string ACCEPT = "Accept";
        public static readonly string DENY = "Deny";
        public static readonly string SUCCESS = "Success";
        public static readonly string CANCEL = "Cancel";
    }
}
