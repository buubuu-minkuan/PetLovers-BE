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
        public Task<PostTradeUserRequestModel> GetRequestPostTrade(Guid postId, Guid userId);
    }
}
