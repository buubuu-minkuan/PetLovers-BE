using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.ResultModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.CommentRepo
{
    public interface ICommentRepo : IRepository<TblPostReaction>
    {
        public Task<CommentResModel> GetCommentById(Guid id);
        public Task<List<CommentResModel>> GetCommentsByPostId(Guid postId);
    }
}
