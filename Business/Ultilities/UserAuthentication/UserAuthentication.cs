//using Data.Entities;
using Data.Models.ResultModel;
using Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business.Services.SecretServices;
using System.Threading.Tasks;

namespace Business.Ultilities.UserAuthentication
{
    public class UserAuthentication
    {
        private static string Key = SecretService.GetJWTKey();
        private static string Issuser = SecretService.GetJWTIssuser();

        public static byte[] CreatePasswordHash(string password)
        {
            using (MD5 mh = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                byte[] hash = mh.ComputeHash(inputBytes);
                return hash;
            }
        }

        public static bool VerifyPasswordHash(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }

            return true;
        }

        public static string GenerateJWT(TblUser UserInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,UserInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,UserInfo.Username.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuser,
                audience: Issuser,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        public static bool ReadJwtToken(string jwtToken, string secretKey, string issuer, ref ResultModel result)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
                var jwtSecurityToken = (JwtSecurityToken)validatedToken;
                string? userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                string? jti = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                result.Data = userId;

                if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                result.Message = "Token has expired";
                return false;
            }
            catch (SecurityTokenValidationException ex)
            {
                result.Message = "Invalid token: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                result.Message = "Error reading token: " + ex.Message;
                return false;
            }
        }
    }
}
