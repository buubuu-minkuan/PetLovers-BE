using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.PostRepo;
using Data.Repositories.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PostServices
{
    public class PostServices : IPostServices
    {
        private readonly IPostRepo _postRepo;
        private readonly UserAuthentication _userAuthentication;

        public PostServices(IPostRepo postRepo)
        {
            _userAuthentication = new UserAuthentication();
            _postRepo = postRepo;
        }
        public async Task<ResultModel> GetPostById(Guid id)
        {
            ResultModel result = new();
            try
            {
                var post = await _postRepo.GetPostById(id);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    result.Code = 200;
                    return result;
                }
                result.IsSuccess = true;
                result.Data = post;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetNewsFeed(string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                var data = await _postRepo.GetNewFeed(userId);
                if (data == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    result.Code = 200;
                    return result;
                }
                result.IsSuccess = true;
                result.Data = data;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreatePost(PostCreateReqModel newPost)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(newPost.token, "userid"));
            Guid postId = Guid.NewGuid();
            TblPost postReq = new()
            {
                Id = postId,
                Type = PostingType.POSTING,
                UserId = userId,
                Status = PostingStatus.PENDING,
                IsProcessed = false,
                Content = newPost.content,
                Attachment = newPost.attachment,
                CreateAt = now
            };
            try
            {
                _ = await _postRepo.Insert(postReq);
                PostResModel postResModel = new()
                {
                    Id = postId,
                    userId = userId,
                    content = newPost.content,
                    attachment = newPost.attachment,
                    createdAt = now,
                    updatedAt = null
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = postResModel;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        /*public async Task<ResultModel> UpdatePost(PostUpdateReqModel post)
        {

        }*/
    }
}
