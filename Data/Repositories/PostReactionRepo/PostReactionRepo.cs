﻿using Data.Entities;
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
            var comments = await _context.TblPostReactions.Where(c => c.PostId.Equals(postId) && c.Type.Equals(ReactionType.COMMENT)).OrderBy(c => c.CreateAt).ToListAsync();
            return comments.Select(c => new CommentResModel
            {
                Id = c.Id,
                UserId = c.UserId,
                PostId = c.PostId,
                Type = c.Type,
                content = c.Content,
                attachment = c.Attachment,
                createdAt = c.CreateAt,
                updatedAt = c.UpdateAt
            }).ToList();
        }

        public async Task<CommentResModel> GetCommentById(Guid id)
        {

            var comment = await _context.TblPostReactions.Where(x => x.Id.Equals(id) && x.Type.Equals(ReactionType.COMMENT)).FirstOrDefaultAsync();
            CommentResModel res = new()
            {
                Id = comment.Id,
                UserId = comment.UserId,
                PostId = comment.PostId,
                Type = comment.Type,
                content = comment.Content,
                attachment = comment.Attachment,
                createdAt = comment.CreateAt,
                updatedAt = comment.UpdateAt
            };
            return res;
        }

        public async Task<TblPostReaction> GetTblPostReactionByPostId(Guid id)
        {
            return await _context.TblPostReactions.Where(x => x.PostId.Equals(id) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
        }

        public async Task<List<TblPostReaction>> GetListReactionById(Guid Id)
        {
            return await _context.TblPostReactions.Where(x => x.PostId.Equals(Id)).ToListAsync();
        }

        public async Task<PostFeelingResModel> GetListFeelingByPostId(Guid postId)
        {
            var feeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(postId) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            PostFeelingResModel res = new();
            res.Id = postId;
            List<FeelingListResModel> listFeeling = new();
            foreach (var feel in feeling)
            {
                var user = await _context.TblUsers.Where(x => x.Id.Equals(feel.UserId)).FirstOrDefaultAsync();
                listFeeling.Add(new FeelingListResModel()
                {
                    Id = feel.Id,
                    Author = new FeelingAuthorModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                    },
                    Type = feel.Type,
                    createdAt = feel.CreateAt
                });
            }
            res.feeling = listFeeling;
            return res;
        }
    }
}
