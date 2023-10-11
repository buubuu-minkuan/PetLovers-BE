//using Data.Entities;
using Data.Models.ResultModel;
using Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business.Services.SecretServices;
using System.Threading.Tasks;
using Data.Models.UserModel;
using System.IdentityModel.Tokens.Jwt;

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

        public static string GenerateJWT(TblUser User)
        {
            UserModel UserInfo = new UserModel()
            {
                Id = User.Id,
                Username = User.Username,
                Name = User.Name,
                RoleId = User.RoleId,
                Status = User.Status,
                Email = User.Email,
                Phone = User.Phone,
                CreateAt = User.CreateAt
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new()
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, UserInfo.RoleId.ToString()),
                new Claim("userid", UserInfo.Id.ToString()),
                new Claim("username", UserInfo.Username),
                new Claim("email", UserInfo.Email),
            };

            var token = new JwtSecurityToken(
                issuer: Issuser,
                audience: Issuser,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        public static bool ReadJwtToken(string jwtToken, ref ResultModel result)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)),
                ValidateIssuer = true,
                ValidIssuer = Issuser,
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
