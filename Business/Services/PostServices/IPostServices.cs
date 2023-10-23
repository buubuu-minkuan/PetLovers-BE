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
        public Task<ResultModel> GetPostById(Guid id);
        public Task<ResultModel> GetNewsFeed(string token);
        public Task<ResultModel> CreatePost(PostCreateReqModel newPost);
        public Task<ResultModel> UpdatePost(PostUpdateReqModel post);
        public Task<ResultModel> DeletePost(PostReqModel post);
        public Task<ResultModel> StorePost(PostReqModel post);
        public Task<ResultModel> RemoveStorePost(PostStoreReqModel post);
        public Task<ResultModel> GetAllPendingPost(string token);
        public Task<ResultModel> GetUserPendingPost(string token);
        public Task<ResultModel> ApprovePosting(PostReqModel post);
        public Task<ResultModel> RefusePosting(PostReqModel post);
    }
}
