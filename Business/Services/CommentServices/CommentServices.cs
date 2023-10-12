using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.ResultModel;
using Business.Ultilities.UserAuthentication;
using Data.Models.UserModel;
using Data.Repositories.CommentRepo;
using Data.Repositories.PostRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.CommentServices
{
    public class CommentServices : ICommentServices //ko biết sao lỗi
    {
        private readonly ICommentRepo _commentRepo;
        private readonly UserAuthentication _userAuthentication;
        public CommentServices(ICommentRepo CommentRepo)
        {
            _userAuthentication = new UserAuthentication();
            _commentRepo = CommentRepo;
        }
        public async Task<ResultModel> GetCommentById(Guid id)
        {
            ResultModel result = new();
            CommentResModel commentResModel = await _commentRepo.GetCommentById(id);
            if (commentResModel == null)
            {
                result.IsSuccess = false;
                result.Code = 200;
                result.Message = "Not Found";
                return result;
            }
            result.Code = 200;
            result.Data = commentResModel;
            result.IsSuccess = true;
            return result;
            
        }
        public async Task<ResultModel> GetCommentsForPost(Guid postId)
        {
            ResultModel result = new();
            List<CommentResModel> comments = (List<CommentResModel>) await _commentRepo.GetCommentsByPostId(postId);
            result.IsSuccess = true;
            result.Code = 200;
            result.Data = comments;
            return result; 
        }

        public async Task<ResultModel> CreateComment(CommentCreateResModel newComment)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid commentId = Guid.NewGuid();
            TblPostReaction commentReq = new()
            {
                Id = commentId,
                PostId = newComment.postId,
                Type = "Comment",
                UserId = new Guid(_userAuthentication.decodeToken(newComment.token, "userid")),
                Content = newComment.content,
                Attachment = newComment.attachment,
                CreateAt = now
            };
            try
            {
                _ = await _commentRepo.Insert(commentReq);
                CommentResModel commentResModel = new CommentResModel();
                commentResModel.Id = commentId;
                commentResModel.content = newComment.content;
                commentResModel.attachment = newComment.attachment;
                commentResModel.createdAt = now;
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = commentResModel;
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
