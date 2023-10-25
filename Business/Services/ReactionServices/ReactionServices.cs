using Business.Ultilities.UserAuthentication;
using Data.Models.ResultModel;
using Data.Repositories.PostReactRepo;
using Data.Repositories.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.ReactionServices
{
    public class ReactionServices : IReactionServices
    {
        private readonly IPostReactionRepo _reactionRepo;
        private readonly UserAuthentication _userAuthentication;

        public ReactionServices(IPostReactionRepo reactionRepo)
        {
            _userAuthentication = new UserAuthentication();
            _reactionRepo = reactionRepo;
        }
        public async Task<ResultModel> CreateFeeling()
        {
            ResultModel result = new();
            result.IsSuccess = true;
            result.Code = 200;
            return result;
        }
    }
}
