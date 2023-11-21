using System;
using Data.Models.ResultModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.SearchServices
{
    public interface ISearchServices
    {
        public Task<ResultModel> Search(string keyword, string token);
    }
}
