using Data.Models.PostModel;
using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PostServices
{
    public interface IPostServices
    {
        public Task<ResultModel> GetPostById(Guid id, string token);
        public Task<ResultModel> GetNewsFeed(string token);
        public Task<ResultModel> CreatePost(PostCreateReqModel newPost);
        public Task<ResultModel> UpdatePost(PostUpdateReqModel post);
        public Task<ResultModel> GetPostTradeById(Guid id, string token);
        public Task<ResultModel> CreatePostTrade(PostTradeCreateReqModel newPost);
        public Task<ResultModel> UpdatePostTrade(PostTradeUpdateReqModel postReq);
        public Task<ResultModel> DeletePostTrade(PostDeleteReqModel post);
        public Task<ResultModel> DeletePost(PostReqModel post);
        public Task<ResultModel> StorePost(Guid postId, string token);
        public Task<ResultModel> RemoveStorePost(Guid postId, string token);
        public Task<ResultModel> RequestTrading(Guid postId, string token);
        public Task<ResultModel> AcceptTrading(PostTradeProcessModel req, string token);
        public Task<ResultModel> DenyTrading(PostTradeProcessModel req, string token);
        public Task<ResultModel> DoneTradingForAuthor(PostTradeProcessModel req, string token);
        public Task<ResultModel> DoneTradingForUser(PostTradeProcessModel req, string token);
        public Task<ResultModel> CancelTrading(PostTradeProcessModel req, string token);
        public Task<ResultModel> GetUserPendingPost(string token);
        public Task<ResultModel> ReportPost(PostReportModel post);
        public Task<ResultModel> GetAllTradePostsTitle();
        public Task<ResultModel> GetListPostTradeByUserId(Guid id, string token);//
        public Task<ResultModel> GetListPostTradeRequested(string token);
    }
}
