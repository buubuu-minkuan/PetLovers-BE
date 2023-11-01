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
            _userFollowingService = userFollowingService;regfewswfesw
        }
        [HttpPost("follow-user")]
        public async Task<IActionResult> FollowUser([FromBody] UserFollowingModel userFollowing)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            userFollowing.token = token;
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.FollowUser(userFollowing);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] UserFollowingModel userFollowing)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            userFollowing.token = token;
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.UnFollowUser(userFollowing);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-follower")]
        public async Task<IActionResult> GetFollowers(Guid userId)
        {
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.GetFollowers(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-following")]
        public async Task<IActionResult> GetFollowings(Guid userId)
        {
            Data.Models.ResultModel.ResultModel result = await _userFollowingService.GetFollowings(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
    }

