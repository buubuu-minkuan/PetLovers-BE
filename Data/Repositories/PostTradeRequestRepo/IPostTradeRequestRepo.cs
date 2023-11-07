using Data.Entities;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostTradeRequestRepo
{
    public interface IPostTradeRequestRepo : IRepository<TblTradeRequest>
    {
        public Task<List<PostTradeUserRequestModel>> GetListRequestPostTradeByPostId(Guid postId);
        public Task<TblTradeRequest> GetRequestPostTrade(Guid postId, Guid userId);
        public Task<List<TblTradeRequest>> GetListRequestCancelByAuthor(Guid postId);
        public Task<List<PostTradeTitleModel>> GetListPostTradeRequested(Guid userId);
    }
}
