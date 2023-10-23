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
        public Task<PostResModel> GetPostById(Guid id);
        public Task<List<PostResModel>> GetPostsFromFollow(Guid userId);
        public Task<List<PostResModel>> GetAllPosts();
        public Task<TblPost> GetTblPostById(Guid id);
        public Task<List<PostResModel>> GetAllPendingPost();
        public Task<List<PostResModel>> GetUserPendingPost(Guid userId);
    }
}
