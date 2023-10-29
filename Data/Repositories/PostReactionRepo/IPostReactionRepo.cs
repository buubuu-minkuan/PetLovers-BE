using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;

namespace Data.Repositories.PostReactRepo
{
    public interface IPostReactionRepo : IRepository<TblPostReaction>
    {
        Task<List<TblPostReaction>> GetListReactionById(Guid id);
        Task<CommentResModel> GetCommentById(Guid id);
        Task<List<CommentResModel>> GetCommentsByPostId(Guid postId);
        Task<TblPostReaction> GetTblPostReactionByPostId(Guid id);
        Task<PostFeelingResModel> GetListFeelingByPostId(Guid postId);
    }

}
