using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Business.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Data.Models.PostModel;
using Business.Services.ManageServices;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Hosting;

namespace API.Controllers
{
    [Route("manage/")]
    [Authorize]
    [ApiController]
    public class ManageController : Controller
    {
        private readonly IManageServices _manage;

        public ManageController(IManageServices manage)
        {
            _manage = manage;
        }

        [HttpGet("post/pending-post")]
        public async Task<IActionResult> GetAllPendingPost()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetAllPendingPost(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("post/approve-post")]
        public async Task<IActionResult> ApprovePost([FromBody] PostReqModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Post.token = token;
            Data.Models.ResultModel.ResultModel result = await _manage.ApprovePosting(Post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("post/refuse-post")]
        public async Task<IActionResult> RefusePost([FromBody] PostReqModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Post.token = token;
            Data.Models.ResultModel.ResultModel result = await _manage.RefusePosting(Post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("post/ban-user")]
        public async Task<IActionResult> BanUser(List<Guid> userId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.BanUser(userId,token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/count-post-approve")]
        public async Task<IActionResult> GetPostApproveForAdmin()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetPostApproveForAdmin(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/count-post-trade")]
        public async Task<IActionResult> GetPostTradeForAdmin()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetPostTradeForAdmin(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/count-post-trade-done")]
        public async Task<IActionResult> GetPostTradeDoneForAdmin()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetPostTradeDoneForAdmin(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/count-post-post-trade-day-week-month")]
        public async Task<IActionResult> CountPostAndPostTradeDayWeekMonthForAdmin()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.CountPostAndPostTradeDayWeekMonthForAdmin(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/get-list-report-post-for-staff")]
        public async Task<IActionResult> GetListReportPostForStaff()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetListReportPostForStaff(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("post/get-list-user-for-admin")]
        public async Task<IActionResult> GetListUserForAdmin()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.GetListUser(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("post/set-staff")]
        public async Task<IActionResult> SetStaff(Guid userId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.SetStaff(userId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("post/remove-staff")]
        public async Task<IActionResult> RemoveStaff(Guid userId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.RemoveStaff(userId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("post/delete-post-by-staff")]
        public async Task<IActionResult> DeletePostByStaff([FromBody] PostReqModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _manage.DeletePostByStaff(Post, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}