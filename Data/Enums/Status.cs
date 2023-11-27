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
        public static readonly string VERIFYING = "Verifying";
        public static readonly string RESETPASSWORD = "ResetPassword";
        public static readonly string DEACTIVE = "Deactive";
        public static readonly string TIMEOUT = "TimeOut";
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
        public static readonly string DEACTIVE = "Deactive";
        public static readonly string WAITINGDONEBYUSER = "WaitingDoneByUser";
        public static readonly string WAITINGDONEBYAUTHOR = "WaitingDoneByAuthor";
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
        public static readonly string CANCELBYUSER = "CancelByUser";
        public static readonly string CANCELBYAUTHOR = "CancelByAuthor";
    }
    public class TypeTrading
    {
        public static readonly string GIFT = "Gift";
        public static readonly string TRADE = "Trade";
        public static readonly string SELL = "Sell";
    }
}
