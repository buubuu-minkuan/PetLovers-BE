using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.UserRepo;
using Data.Repositories.UserRewardRepo;
using System.Security.Claims;

namespace Business.Services.ManageServices
{
    public class ManageServices : IManageServices
    {
        private readonly UserAuthentication _userAuthentication;
        private readonly IUserRepo _userRepo;
        private readonly IPostRepo _postRepo;
        private readonly IUserRewardRepo _rewardRepo;
        private readonly IPostAttachmentRepo _postAttachmentRepo;

        public ManageServices(IUserRepo userRepo, IPostRepo postRepo, IUserRewardRepo rewardRepo, IPostAttachmentRepo postAttachmentRepo)
        {
            _userAuthentication = new UserAuthentication();
            _userRepo = userRepo;
            _postRepo = postRepo;
            _rewardRepo = rewardRepo;
            _postAttachmentRepo = postAttachmentRepo;
        }

        public async Task<ResultModel> BanUser(List<Guid> userId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid modId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                foreach (var id in userId)
                {
                    var user = await _userRepo.Get(id);
                    user.Status = UserStatus.DEACTIVE;
                    user.UpdateAt = now;
                    _ = await _userRepo.Update(user);
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
        public async Task<ResultModel> GetAllPendingPost(string token)
        {
            ResultModel result = new();
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                List<PostResModel> listPost = await _postRepo.GetAllPendingPost();
                result.Code = 200;
                result.IsSuccess = true;
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
        public async Task<ResultModel> ApprovePosting(PostReqModel post)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(post.token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(post.token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                foreach (var pReq in post.postId)
                {
                    TblPost getPost = await _postRepo.GetTblPostById(pReq);
                    if (getPost.ModeratorId != null && getPost.IsProcessed)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "The post is processed by other moderator!";
                        return result;
                    }
                    var totalPost = await _postRepo.GetPostsApproveByUserId(userId);
                    var listReward = await _rewardRepo.GetListPostReward();
                    foreach (var reward in listReward)
                    {
                        if (totalPost.Count >= reward.TotalPost)
                        {
                            TblUserReward userReward = new()
                            {
                                UserId = getPost.UserId,
                                RewardId = reward.Id,
                                Status = Status.ACTIVE,
                                CreateAt = now
                            };
                            _ = await _rewardRepo.Insert(userReward);
                        }
                    }
                    getPost.Status = PostingStatus.APPROVED;
                    getPost.ModeratorId = userId;
                    getPost.IsProcessed = true;
                    _ = await _postRepo.Update(getPost);
                    result.Code = 200;
                    result.IsSuccess = true;
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
        public async Task<ResultModel> RefusePosting(PostReqModel post)
        {
            ResultModel result = new();
            Guid roleId = new Guid(_userAuthentication.decodeToken(post.token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.STAFF))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                foreach (var pReq in post.postId)
                {
                    TblPost getPost = await _postRepo.GetTblPostById(pReq);
                    if (getPost.ModeratorId != null && getPost.IsProcessed)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "The post is processed by other moderator!";
                        return result;
                    }
                    List<TblPostAttachment> attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(pReq);
                    foreach (var attachment in attachments)
                    {
                        attachment.Status = Status.DEACTIVE;
                        _ = await _postAttachmentRepo.Update(attachment);
                    }
                    getPost.Status = PostingStatus.REFUSED;
                    getPost.IsProcessed = true;
                    _ = await _postRepo.Update(getPost);
                    result.Code = 200;
                    result.IsSuccess = true;
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
    }
}