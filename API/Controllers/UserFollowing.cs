using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Business.Services.UserFollowingServices;
using Data.Models.UserModel;
using Business.Services.UserFollowingServices;
using Microsoft.Extensions.Hosting;

namespace API.Controllers
{
    [Route("UserFollowing/")]
    [ApiController]
    public class UserFollowingController : Controller
    {
        private readonly IUserFollowingServices _userFollowingService;

        public UserFollowingController(IUserFollowingServices userFollowingService)
        {
            _userFollowingService = userFollowingService;
        }
        [HttpPost("follow-user")]
        public async Task<IActionResult> FollowUser([FromBody] UserFollowingModel userFollowing)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            userFollowing.token = token;
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.FollowUser(userFollowing);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        /*[HttpGet("is-following")]
        public async Task<IActionResult> IsFollowing(Guid userId, Guid followerId)
        {
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.IsFollowing(userId, followerId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("unfollow-user")]
        public async Task<IActionResult> UnfollowUser([FromBody] UserFollowingModel userFollowing)
        {
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.UnfollowUser(userId, followerId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }*/
    }
        
}