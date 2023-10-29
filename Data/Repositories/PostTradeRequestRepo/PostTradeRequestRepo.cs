using Data.Entities;
using Data.Enums;
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
            var listReq = await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId)).ToListAsync();
            List<PostTradeUserRequestModel> res = new();
            foreach (var req in listReq)
            {
                PostTradeUserRequestModel userReq = new()
                {
                    Id = req.Id,
                    UserId = req.UserId,
                    Status = req.Status,
                    createdAt = req.CreateAt
                };
                res.Add(userReq);
            }
            return res;
        }

        public async Task<PostTradeUserRequestModel> GetRequestPostTrade(Guid postId, Guid userId)
        {
            var req = await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId) && x.UserId.Equals(userId)).FirstOrDefaultAsync();
            PostTradeUserRequestModel userReq = new()
            {
                Id = req.Id,
                UserId = req.UserId,
                Status = req.Status,
                createdAt = req.CreateAt
            };
            return userReq;
        }
        public async Task<List<TblTradeRequest>> GetListRequestCancelByAuthor(Guid postId)
        {
            return await _context.TblTradeRequests.Where(x => x.PostId.Equals(postId) && x.Status.Equals(TradeRequestStatus.CANCELBYAUTHOR)).ToListAsync();
        }
    }
}
