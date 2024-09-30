using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoListAPI.Models;

using ToDoListAPI.Interface;

namespace ToDoListAPI.Helper
{
    public class ToDoHelper
    {
        private IConfiguration _IConfiguration;
        public ToDoHelper(IConfiguration configuration)
        {
            _IConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public string GenerateToken(string username)
        {
            var jwtSettings = _IConfiguration.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("Key")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetValue<string>("Issuer"),
                audience: jwtSettings.GetValue<string>("Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
