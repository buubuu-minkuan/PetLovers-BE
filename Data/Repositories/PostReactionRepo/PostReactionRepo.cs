using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.FeelingModel;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostReactRepo
{
    public class PostReactionRepo : Repository<TblPostReaction>, IPostReactionRepo
    {
        private readonly PetLoversDbContext _context;
        public PostReactionRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<CommentResModel>> GetCommentsByPostId(Guid postId)
        {
            var comments = await _context.TblPostReactions.Where(c => c.PostId.Equals(postId) && c.Type.Equals(ReactionType.COMMENT) && c.Status.Equals(Status.ACTIVE)).OrderBy(c => c.CreateAt).ToListAsync();
            List<CommentResModel> listComment = new();
            foreach(var comment in comments)
            {
                var user = await _context.TblUsers.Where(x => x.Id.Equals(comment.UserId)).FirstOrDefaultAsync();
                CommentAuthor author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };
                CommentResModel commentModel = new()
                {
                    Id = comment.Id,
                    Author = author,
                    PostId = comment.PostId,
                    Type = comment.Type,
                    content = comment.Content,
                    attachment = comment.Attachment,
                    createdAt = comment.CreateAt,
                    updatedAt = comment.UpdateAt
                };
                listComment.Add(commentModel);
            }
            return listComment;
        }

        public async Task<CommentResModel> GetCommentById(Guid id)
        {

            var comment = await _context.TblPostReactions.Where(x => x.Id.Equals(id) && x.Type.Equals(ReactionType.COMMENT)).FirstOrDefaultAsync();
            var user = await _context.TblUsers.Where(x => x.Id.Equals(comment.UserId)).FirstOrDefaultAsync();
            CommentAuthor author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image
            };
            CommentResModel res = new()
            {
                Id = comment.Id,
                Author = author,
                PostId = comment.PostId,
                Type = comment.Type,
                content = comment.Content,
                attachment = comment.Attachment,
                createdAt = comment.CreateAt,
                updatedAt = comment.UpdateAt
            };
            return res;
        }

        public async Task<TblPostReaction> GetTblPostReactionByReactionId(Guid id)
        {
            return await _context.TblPostReactions.Where(x => x.Id.Equals(id) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
        }

        public async Task<List<TblPostReaction>> GetListReactionById(Guid Id)
        {
            return await _context.TblPostReactions.Where(x => x.PostId.Equals(Id)).ToListAsync();
        }

        public async Task<TblPostReaction> isFeeling(Guid postId, Guid userId)
        {
            return await _context.TblPostReactions.Where(x => x.UserId.Equals(userId) && x.PostId.Equals(postId) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
        }
    }
}
