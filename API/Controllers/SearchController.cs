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
using Microsoft.AspNetCore.Authorization;
using Data.Models.OTPVerifyModel;

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

        [HttpPost("send-verify-email")]
        [Authorize]
        public async Task<IActionResult> SendVerifyEmailOTP()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _email.SendVerifyEmailOTP(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("send-verify-reset-password")]
        public async Task<IActionResult> SendVerifyResetPassword([FromBody] string email)
        {
            Data.Models.ResultModel.ResultModel result = await _email.SendVerifyResetPassword(email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("otp")]
        public async Task<IActionResult> Verify([FromBody] OTPVerifyReqModel OTPCode)
        {
            Data.Models.ResultModel.ResultModel result = await _email.VerifyOTPCode(OTPCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}