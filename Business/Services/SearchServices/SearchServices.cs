using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models.ResultModel;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.PostRepo;
using Data.Repositories.UserRepo;
using Data.Repositories.HashtagRepo;
using Business.Ultilities.UserAuthentication;
using Data.Models.SearchModel;

namespace Business.Services.SearchServices
{
    public class SearchServices : ISearchServices
    {
        private readonly IPostRepo _postRepository;
        private readonly IUserRepo _userRepository;
        private readonly IHashtagRepo _hashtagRepository;
        private readonly UserAuthentication _userAuthentication;

        public SearchServices(IPostRepo postRepository, IUserRepo userRepository, IHashtagRepo hashtagRepository)
        {
            _postRepository = postRepository;
            _userAuthentication = new UserAuthentication();
            _userRepository = userRepository;
            _hashtagRepository = hashtagRepository;
        }

        public async Task<ResultModel> Search(string keyword, string token)
        {
            ResultModel result = new();
            SearchResModel searchRes = new SearchResModel();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var user = await _userRepository.SearchUser(keyword, userId);
                var post = await _postRepository.SearchPost(keyword, userId);
                var hashtag = await _hashtagRepository.SearchHashtag(keyword);
                searchRes.Users = user;
                searchRes.Posts = post;
                searchRes.Hashtag = hashtag;
                return result = new ResultModel()
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = searchRes
                };
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
