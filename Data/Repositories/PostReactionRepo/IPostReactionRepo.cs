using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;

namespace Data.Repositories.PostReactRepo
{
    public interface IPostReactionRepo : IRepository<TblPostReaction>
    {
        public Task<List<TblPostReaction>> GetListReactionById(Guid id);
        public Task<CommentResModel> GetCommentById(Guid id);
        public Task<List<CommentResModel>> GetCommentsByPostId(Guid postId);
        public Task<TblPostReaction> GetTblPostReactionByPostId(Guid id);
        public Task<TblPostReaction> isFeeling(Guid postId, Guid userId);
    }

}
