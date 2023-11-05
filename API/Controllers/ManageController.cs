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
    }
}