using Business.Ultilities.UserAuthentication;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Repositories.HashtagRepo;
using Data.Repositories.PostRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.HashtagServices
{
    public class HashtagServices : IHashtagServices
    {
        private readonly IHashtagRepo _hashtagRepo;
        private readonly IPostRepo _postRepo;
        private readonly UserAuthentication _userAuthentication;

        public HashtagServices(IHashtagRepo hashtagRepo, IPostRepo postRepo)
        {
            _postRepo = postRepo;
            _hashtagRepo = hashtagRepo;
            _userAuthentication = new UserAuthentication();
        }

        public async Task<ResultModel> GetHashtagTrending()
        {
            ResultModel result = new();
            try
            {
                var listHashtag = await _hashtagRepo.GetListHashtagTrending();
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listHashtag;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetListPostsByHashtag(string hashtag, string token)
        {
            ResultModel result = new();
            try
            {
                var listPostByHashtag = await _hashtagRepo.GetListPostIdByHashtag(hashtag);
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var listPost = new List<PostResModel>();
                foreach (var post in listPostByHashtag)
                {
                    var getPost = await _postRepo.GetPostById(post, userId);
                    listPost.Add(getPost);
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = listPost;
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
