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
using Newtonsoft.Json.Linq;

namespace Business.Ultilities.UserAuthentication
{

    public class UserAuthentication
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private static string Key = SecretService.GetJWTKey();
        private static string Issuser = SecretService.GetJWTIssuser();

        public UserAuthentication()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

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

        public static string GenerateJWT(UserModel User)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new()
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role.Id.ToString()),
                new Claim("userid", User.Id.ToString()),
                new Claim("username", User.Username),
                new Claim("email", User.Email),
            };

            var token = new JwtSecurityToken(
                issuer: Issuser,
                audience: Issuser,
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: credential
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        public string decodeToken(string jwtToken, string nameClaim)
        {
            Claim? claim = _tokenHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            return claim != null ? claim.Value : "Error!!!";
        }
    }
}
