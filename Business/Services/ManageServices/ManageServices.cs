using Business.Ultilities.EmailNotification;
using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.HashtagRepo;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.ReportRepo;
using Data.Repositories.UserRepo;
using Data.Repositories.UserRewardRepo;
using Newtonsoft.Json.Linq;
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
        private readonly IHashtagRepo _hashtagRepo;
        private readonly IPostReactionRepo _postReactionRepo;
        private readonly IReportRepo _reportRepo;
        private readonly EmailNotification _emailNotification;


        public ManageServices(IUserRepo userRepo, IPostRepo postRepo, IUserRewardRepo rewardRepo, IPostAttachmentRepo postAttachmentRepo, IHashtagRepo hashtagRepo, IPostReactionRepo postReactionRepo, IReportRepo reportRepo)
        {
            _userAuthentication = new UserAuthentication();
            _emailNotification = new EmailNotification();
            _userRepo = userRepo;
            _postRepo = postRepo;
            _rewardRepo = rewardRepo;
            _postAttachmentRepo = postAttachmentRepo;
            _hashtagRepo = hashtagRepo;
            _postReactionRepo = postReactionRepo;
            _reportRepo = reportRepo;
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
                if (roleName.Equals(Commons.STAFF) && roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                foreach (var id in userId)
                {
                    var user = await _userRepo.Get(id);
                    user.Status = UserStatus.TIMEOUT;
                    user.UpdateAt = now;
                    _ = await _userRepo.Update(user);
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
            Guid modId = new Guid(_userAuthentication.decodeToken(post.token, "userid"));
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
                    var getuser = await _userRepo.Get(getPost.UserId);
                    var getmod = await _userRepo.Get(modId);
                    if(getuser == null)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "User is not exist";
                        return result;
                    }
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
                    bool check = await _emailNotification.SendRefusePostNotification(getuser.Email, post.reason, "Bài viết đang chờ được duyệt của bạn đã bị từ chối bởi: <B>" + getmod.Name + "</B>");
                    if (check)
                    {
                        result.Message = "Send Email Successfully!";
                    }else
                    {
                        result.Message = "Cann't Send Email!";
                    } 
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
        public async Task<ResultModel> GetPostApproveForAdmin(string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var totalPostTrade = await _postRepo.GetAllPosts(userId);
                int count = totalPostTrade.Count;
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = count;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetPostTradeForAdmin(string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var totalPostTrade = await _postRepo.GetAllTradePostsTitle();
                int count = totalPostTrade.Count;
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = count;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetPostTradeDoneForAdmin(string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var totalPostTrade = await _postRepo.GetAllTradePostsDone();
                int count = totalPostTrade.Count;
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = count;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> CountPostAndPostTradeDayWeekMonthForAdmin(string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                int countDailyPost = await _postRepo.CountDailyPost(now);
                int countDailyPostTrade = await _postRepo.CountDailyPostTrade(now);
                int countWeeklyPost = await _postRepo.CountWeeklyPost(now);
                int countWeeklyPostTrade = await _postRepo.CountWeeklyPostTrade(now);
                int countMonthlyPost = await _postRepo.CountMonthlyPost(now);
                int countMonthlyPostTrade = await _postRepo.CountMonthlyPostTrade(now);

                var countModel = new CountPostAndPostTradeForAdmin
                {
                    CountDailyPost = countDailyPost.ToString(),
                    CountDailyPostTrade = countDailyPostTrade.ToString(),
                    CountWeeklyPost = countWeeklyPost.ToString(),
                    CountWeeklyPostTrade = countWeeklyPostTrade.ToString(),
                    CountMonthlyPost = countMonthlyPost.ToString(),
                    CountMonthlyPostTrade = countMonthlyPostTrade.ToString()
                };
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = countModel;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetListReportPostForStaff(string token)
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
                var post = await _postRepo.GetListReportPostForStaff();
                result.IsSuccess = true;
                result.Data = post;
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
        public async Task<ResultModel> SetStaff(Guid userId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid modId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var getRoleStaffId = await _userRepo.GetRoleId(Commons.STAFF);
                var user = await _userRepo.Get(userId);
                if (!user.RoleId.Equals(getRoleStaffId))
                {
                    user.RoleId = getRoleStaffId;
                    user.UpdateAt = now;
                    _ = await _userRepo.Update(user);
                    result.Code = 200;
                    result.IsSuccess = true;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "This User Is Already A Staff";
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
        public async Task<ResultModel> RemoveStaff(Guid userId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid modId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var getRoleUserId = await _userRepo.GetRoleId(Commons.USER);
                var user = await _userRepo.Get(userId);
                if (!user.RoleId.Equals(getRoleUserId))
                {
                    user.RoleId = getRoleUserId;
                    user.UpdateAt = now;
                    _ = await _userRepo.Update(user);
                    result.Code = 200;
                    result.IsSuccess = true;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "This User Is Already A User";
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
        public async Task<ResultModel> GetListUser(string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid modId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, ClaimsIdentity.DefaultRoleClaimType));
            string roleName = await _userRepo.GetRoleName(roleId);
            try
            {
                if (!roleName.Equals(Commons.ADMIN))
                {
                    result.Code = 403;
                    result.IsSuccess = false;
                    result.Message = "User role invalid";
                    return result;
                }
                var getListUser = await _userRepo.GetListUserForAdmin();
                var orderedList = getListUser.OrderBy(user => user.RoleName != Commons.STAFF).ToList();
                result.Code = 200;
                result.Data = orderedList;
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
        public async Task<ResultModel> DeletePostByStaff(PostReqModel postReq, string token)
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
                }else foreach (var pReq in postReq.postId) 
                {
                        
                    TblPost tblPost = await _postRepo.GetTblPostById(pReq);
                    tblPost.UpdateAt = now;
                    tblPost.Status = Status.DEACTIVE;
                    _ = await _postRepo.Update(tblPost);
                    var getUser = await _userRepo.Get(tblPost.UserId);
                    var getmod = await _userRepo.Get(modId);
                    List<TblPostHashtag> Hashtags = await _hashtagRepo.GetListHashTagByPostId(pReq);
                    List<TblPostAttachment> Attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(pReq);
                    List<TblPostReaction> Reactions = await _postReactionRepo.GetListReactionById(pReq);
                    List<TblReport> Reports = await _reportRepo.GetlistTblReport(pReq);
                    foreach (var attachment in Attachments)
                    {
                        attachment.Status = Status.DEACTIVE;
                        _ = await _postAttachmentRepo.Update(attachment);
                    }

                    foreach (var reaction in Reactions)
                    {
                        reaction.Status = Status.DEACTIVE;
                        _ = await _postReactionRepo.Update(reaction);
                    }

                    foreach (var hashtag in Hashtags)
                    {
                        hashtag.Status = Status.DEACTIVE;
                        _ = await _hashtagRepo.Update(hashtag);
                    }
                    foreach (var report in Reports)
                    {
                            report.Status = ReportingStatus.COMPLETE;
                            report.ModeratorId = modId;
                            report.IsProcessed = true;
                            report.UpdatedAt = now;
                            _ = await _reportRepo.Update(report);
                    }
                    _ = await _emailNotification.SendRefusePostNotification(getUser.Email, postReq.reason, "Bài viết của bạn đã bị xóa bởi: <B>" + getmod.Name + "</B>");
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
        public async Task<ResultModel> RefuseReport(Guid postId, string token)
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
                var getReports = await _reportRepo.GetlistTblReport(postId);
                foreach (var report in getReports)
                {
                    report.Status = ReportingStatus.COMPLETE;
                    report.ModeratorId = modId;
                    report.IsProcessed = true;
                    report.UpdatedAt = now;
                    _ = await _reportRepo.Update(report);
                }
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