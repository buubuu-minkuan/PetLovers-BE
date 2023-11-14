using Data.Entities;
using Data.Enums;
using Data.Models.PostAttachmentModel;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostTradeRequestRepo
{
    public class PostTradeRequestRepo : Repository<TblTradeRequest>, IPostTradeRequestRepo
    {
        private readonly PetLoversDbContext _context;
        public PostTradeRequestRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<PostTradeUserRequestModel>> GetListRequestPostTradeByPostId(Guid postId)
        {
            var listReq = await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId) && (x.Status.Equals(TradeRequestStatus.PENDING) || x.Status.Equals(TradeRequestStatus.ACCEPT))).ToListAsync();
            List<PostTradeUserRequestModel> res = new();
            foreach (var req in listReq)
            {
                PostTradeUserRequestModel userReq = new()
                {
                    Id = req.Id,
                    UserId = req.UserId,
                    Attachment = req.Attachment,
                    Status = req.Status,
                    createdAt = req.CreateAt
                };
                res.Add(userReq);
            }
            return res;
        }

        public async Task<TblTradeRequest> GetRequestPostTrade(Guid postId, Guid userId)
        {
            return await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId) && x.UserId.Equals(userId)).OrderByDescending(x => x.CreateAt).FirstOrDefaultAsync();
            
        }
        public async Task<List<TblTradeRequest>> GetListRequestCancelByAuthor(Guid postId)
        {
            return await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId) && x.Status.Equals(TradeRequestStatus.CANCELBYAUTHOR)).ToListAsync();
        }

        public async Task<List<PostTradeTitleModel>> GetListPostTradeRequested(Guid userId)
        {
            var requests = await _context.TblTradeRequests.Where(x => x.UserId.Equals(userId) && (x.Status.Equals(TradeRequestStatus.PENDING) || x.Status.Equals(TradeRequestStatus.ACCEPT))).ToListAsync();
            List<PostTradeTitleModel> listPost = new();
            foreach (var req in requests)
            {
                var post = await _context.TblPosts.Where(x => x.Id.Equals(req.PostId)).FirstOrDefaultAsync();
                var user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };

                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
                listPost.Add(new PostTradeTitleModel()
                {
                    Id = post.Id,
                    Author = author,
                    Title = post.Title,
                    Type = post.Type,
                    Amount = post.Amount,
                    Attachment = arrAttachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt,
                    isFree = post.IsFree
                });
            }
            return listPost;
        }

        private async Task<List<PostAttachmentResModel>> GetPostAttachment(Guid postId)
        {
            var listAttachment = await _context.TblPostAttachments.Where(x => x.PostId.Equals(postId) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            List<PostAttachmentResModel> listResAttachement = new List<PostAttachmentResModel>();
            foreach (var postAttachment in listAttachment)
            {
                PostAttachmentResModel attachment = new()
                {
                    Id = postAttachment.Id,
                    Attachment = postAttachment.Attachment,
                    Status = postAttachment.Status,
                };
                listResAttachement.Add(attachment);
            }
            return listResAttachement;
        }
        public async Task<List<PostTradeTitleModel>> GetListPostTradeHistory(Guid userId)
        {
            var requests = await _context.TblTradeRequests.Where(x => x.UserId.Equals(userId) && (x.Status.Equals(TradeRequestStatus.SUCCESS) || x.Status.Equals(TradeRequestStatus.SUCCESS))).ToListAsync();
            List<PostTradeTitleModel> listPost = new();
            foreach (var req in requests)
            {
                var post = await _context.TblPosts.Where(x => x.Id.Equals(req.PostId)).FirstOrDefaultAsync();
                var user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };

                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
                listPost.Add(new PostTradeTitleModel()
                {
                    Id = post.Id,
                    Author = author,
                    Title = post.Title,
                    Type = post.Type,
                    Amount = post.Amount,
                    Attachment = arrAttachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt,
                    isFree = post.IsFree
                });
            }
            return listPost;
        }
    }
}
