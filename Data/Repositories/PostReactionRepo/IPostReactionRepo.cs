using Data.Entities;
using Data.Models.CommentModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostReactRepo
{
    public interface IPostReactionRepo : IRepository<TblPostReaction>
    {
        public Task<List<TblPostReaction>> GetListReactionById(Guid Id);
        public Task<CommentResModel> GetCommentById(Guid id);
        public Task<List<CommentResModel>> GetCommentsByPostId(Guid postId);
        public Task<TblPostReaction> GetTblPostReactionByPostId(Guid id);
    }
}
