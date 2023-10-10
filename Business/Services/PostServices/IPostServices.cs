using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PostServices
{
    public interface IPostServices
    {
        public Task<ResultModel> GetPostById(Guid id);
        public Task<ResultModel> GetNewsFeed(Guid UserId);
    }
}
