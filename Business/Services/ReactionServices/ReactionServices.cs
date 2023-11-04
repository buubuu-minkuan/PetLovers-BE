using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.FeelingModel;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Repositories.PostReactRepo;
using Data.Repositories.UserRepo;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.ReactionServices
{
    public class ReactionServices : IReactionServices //ko biết sao lỗi
    {
        private readonly IPostReactionRepo _reactionRepo;
        private readonly IUserRepo _userRepo;
        private readonly UserAuthentication _userAuthentication;
        public ReactionServices(IPostReactionRepo reactionRepo, IUserRepo userRepo)
        {
            _userAuthentication = new UserAuthentication();
            _reactionRepo = reactionRepo;
            _userRepo = userRepo;
        }
        public async Task<ResultModel> GetCommentById(Guid id)
        {
            ResultModel result = new();
            try
            {

                CommentResModel commentResModel = await _reactionRepo.GetCommentById(id);
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
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }
        public async Task<ResultModel> GetCommentsForPost(Guid postId)
        {
            ResultModel result = new();
            try
            {
                List<CommentResModel> comments = await _reactionRepo.GetCommentsByPostId(postId);
                if (comments == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Not Found";
                    return result;
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = comments;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateComment(CommentCreateReqModel newComment)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(newComment.token, "userid"));
            Guid commentId = Guid.NewGuid();
            TblPostReaction commentReq = new()
            {
                Id = commentId,
                PostId = newComment.postId,
                Type = ReactionType.COMMENT,
                UserId = userId,
                Content = newComment.content,
                Attachment = newComment.attachment,
                Status = Status.ACTIVE,
                CreateAt = now
            };
            try
            {
                 var user = await _userRepo.Get(userId);
                CommentAuthor author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };
                _ = await _reactionRepo.Insert(commentReq);
                CommentResModel commentResModel = new()
                {
                    Id = commentId,
                    Author = author,
                    PostId = newComment.postId,
                    content = newComment.content,
                    attachment = newComment.attachment,
                    createdAt = now,
                };
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

        public async Task<ResultModel> UpdateComment(CommentReqModel Comment)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(Comment.token, "userid"));
            try
            {
                CommentResModel resComment = await _reactionRepo.GetCommentById(Comment.Id);
                TblPostReaction tblPostReaction = await _reactionRepo.GetTblPostReactionByReactionId(Comment.Id);
                if (resComment == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Comment not found";
                    return result;
                }
                else if (!userId.Equals(resComment.Author.Id))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to update this comment";
                    return result;
                }
                if (!string.IsNullOrEmpty(Comment.content) && !resComment.content.Equals(Comment.content))
                {
                    resComment.content = Comment.content;
                    tblPostReaction.Content = Comment.content;
                }
                resComment.attachment = Comment.attachment;
                resComment.updatedAt = now;
                tblPostReaction.Attachment = Comment.attachment;
                tblPostReaction.UpdateAt = now;
                _ = _reactionRepo.Update(tblPostReaction);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = resComment;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> DeleteComment(CommentDeleteReqModel Comment)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(Comment.token, "userid"));
            try
            {
                CommentResModel resComment = await _reactionRepo.GetCommentById(Comment.Id);
                TblPostReaction tblPostReaction = await _reactionRepo.GetTblPostReactionByReactionId(Comment.Id);
                if (resComment == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Comment not found";
                    return result;
                }
                else if (!userId.Equals(resComment.Author.Id))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to delete this comment";
                    return result;
                }
                else
                {
                    tblPostReaction.UpdateAt = now;
                    tblPostReaction.Status = Status.DEACTIVE;
                    _ = await _reactionRepo.Update(tblPostReaction);
                    result.IsSuccess = true;
                    result.Code = 200;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreateFeeling(Guid postId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid feelingId = Guid.NewGuid();
            TblPostReaction newTblFeeling = new()
            {
                Id = feelingId,
                PostId = postId,
                Type = ReactionType.FEELING,
                UserId = userId,
                TypeReact = FeelingType.LIKE,
                Status = Status.ACTIVE,
                CreateAt = now
            };
            try
            {
                _ = await _reactionRepo.Insert(newTblFeeling);
                var user = await _userRepo.GetUserById(userId);
                FeelingResModel Feeling = new()
                {
                    Id = feelingId,
                    Author = new FeelingAuthorModel()
                    {
                        Id = userId,
                        Name = user.Name
                    },
                    postId = postId,
                    Type = FeelingType.LIKE,
                    createdAt = now
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = Feeling;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> RemoveFeeling(Guid postId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var isFeeling = await _reactionRepo.isFeeling(postId, userId);
                if(isFeeling == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You have not liked this post yet";
                    return result;
                }
                isFeeling.Status = Status.DEACTIVE;
                isFeeling.UpdateAt = now;
                _ = await _reactionRepo.Update(isFeeling);
                result.IsSuccess = true;
                result.Code = 200;
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


    
