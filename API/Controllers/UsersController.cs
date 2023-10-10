using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;

namespace API.Controllers
{
    [Route("user/")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserServices _user;

        public UsersController(IUserServices user)
        {
            _user = user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string Name, string Email, string Username, string Password, string Phone)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Register(Name, Email, Username, Password, Phone);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Login(UserName, Password);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("read-jwt")]
        public IActionResult ReadJwt(string jwtToken, string secretkey, string issuer)
        {
            Data.Models.ResultModel.ResultModel result = _user.ReadJWT(jwtToken, secretkey, issuer);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _user.GetUser(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}