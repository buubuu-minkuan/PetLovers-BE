using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.ResultModel;
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

        public CommentServices(ICommentRepo CommentRepo)
        {
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
    }
}
