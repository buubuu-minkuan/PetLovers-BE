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
        public Task<ResultModel> DeletePost(PostDeleteReqModel post);
        public Task<ResultModel> StorePost(PostStoreReqModel post);
        public Task<ResultModel> RemoveStorePost(PostStoreReqModel post);
    }
}
