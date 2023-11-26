using Data.Entities;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.ManageServices
{
    public interface IManageServices
    {
        public Task<ResultModel> BanUser(List<Guid> userId, string token);
        public Task<ResultModel> GetAllPendingPost(string token);
        public Task<ResultModel> ApprovePosting(PostReqModel post);
        public Task<ResultModel> RefusePosting(PostReqModel post);
        public Task<ResultModel> GetPostApproveForAdmin(string token);
        public Task<ResultModel> GetPostTradeForAdmin(string token);
        public Task<ResultModel> GetPostTradeDoneForAdmin(string token);
        public Task<ResultModel> CountPostAndPostTradeDayWeekMonthForAdmin(string token);
        public Task<ResultModel> GetListReportPostForStaff(string token);
        public Task<ResultModel> SetStaff(Guid userId, string token);
        public Task<ResultModel> RemoveStaff(Guid userId, string token);
        public Task<ResultModel> GetListUser(string token);
        public Task<ResultModel> DeletePostByStaff(PostReqModel postReq, string token);
        public Task<ResultModel> RefuseReport(Guid postId, string token);
    }
}
