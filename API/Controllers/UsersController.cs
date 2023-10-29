using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Data.Models.UserModel;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("user/")]
    public class UsersController : Controller
    {
        private readonly IUserServices _user;

        public UsersController(IUserServices user)
        {
            _user = user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserResgisterModel User)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Register(User);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel User)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Login(User);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _user.GetUser(id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-role-name")]
        [Authorize]
        public async Task<IActionResult> GetRoleName(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _user.GetRoleName(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserUpdateReqModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _user.UpdateUser(model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(UserChangePasswordModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _user.ChangePassword(model, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}