﻿using Data.Entities;
using Data.Models.CommentModel;
using Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.CommentServices
{
    public interface ICommentServices
    {
        public Task<ResultModel> GetCommentById(Guid id);
        public Task<ResultModel> GetCommentsForPost(Guid postId);
        public Task<ResultModel> CreateComment(CommentCreateReqModel newComment);
        public Task<ResultModel> UpdateComment(CommentReqModel Comment);
        public Task<ResultModel> DeleteComment(CommentDeleteReqModel Comment);
    }
}
