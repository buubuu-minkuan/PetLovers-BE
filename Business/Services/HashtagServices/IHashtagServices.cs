using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.HashtagServices
{
    public interface IHashtagServices
    {
        public Task<ResultModel> GetHashtagTrending();
        public Task<ResultModel> GetListPostsByHashtag(string hashtag, string token);
    }
}
