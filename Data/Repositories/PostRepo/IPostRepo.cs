using Data.Entities;
using Data.Models.PostModel;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostRepo
{
    public interface IPostRepo : IRepository<TblPost>
    {
        public Task<PostResModel> GetPostById(Guid id, Guid userId);
        public Task<List<PostResModel>> GetPostsFromFollow(Guid userId);
        public Task<List<PostResModel>> GetPostsFromUser(Guid userId);
        public Task<List<PostResModel>> GetAllPosts(Guid userId);
        public Task<List<TblPost>> GetPostsApproveByUserId(Guid userId);
        public Task<TblPost> GetTblPostById(Guid id);
        public Task<TblPost> GetTblPostTradeById(Guid id);
        public Task<PostTradeResModel> GetPostTradeById(Guid id);
        public Task<List<PostTradeResModel>> GetAllTradePostsTitle();
        public Task<List<PostResModel>> GetAllPendingPost();
        public Task<List<PostResModel>> GetUserPendingPost(Guid userId);
        public Task<List<PostTradeResModel>> GetPostTradingInProgressByUserId(Guid userId);
        public Task<List<TblPost>> GetListPostTradingByUserId(Guid userId);
    }
}
