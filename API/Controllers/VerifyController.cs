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
using Business.Services.VerifyServices;

namespace API.Controllers
{
    [Route("verify/")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IVerifyServices _email;

        public EmailController(IVerifyServices email)
        {
            _email = email;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _email.SendMail(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}