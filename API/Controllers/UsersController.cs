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
        public async Task<IActionResult> GetUser(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _user.GetUser(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}