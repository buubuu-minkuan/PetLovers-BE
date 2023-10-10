using Data.Entities;
using Data.Models.CommentModel;
using Data.Repositories.GenericRepository;
using Data.Repositories.PostRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.CommentRepo
{
    public class CommentRepo : Repository<TblPostReaction>, ICommentRepo
    {
        private readonly PetLoversDbContext _context;
        public CommentRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<CommentResModel> GetCommentById(Guid id)
        {

            var comment = await _context.TblPostReactions.Where(x => x.Id.Equals(id) && x.Type.Equals("Comment")).FirstOrDefaultAsync();
            CommentResModel res = new()
            {
                Id = comment.Id,
                content = comment.Content,
                attachment = comment.Attachment
            };
            return res;
        }
        public async Task<List<CommentResModel>> GetCommentsByPostId(Guid postId)
        {
            var comments = await _context.TblPostReactions.Where(c => c.PostId.Equals(postId) && c.Type.Equals("Comment")).OrderBy(c => c.CreateAt).ToListAsync();
                return comments.Select(c => new CommentResModel
            {
                Id = c.Id,
                content = c.Content,
                attachment = c.Attachment,
                createdAt = c.CreateAt
            }).ToList();
        }
    }
}