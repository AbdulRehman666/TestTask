using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestTask.Models;

namespace TestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // Static list of users (Replace with your actual user data)
        private static readonly List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "ali", Password = "ali123", Role = "Doctor" },
            new User { Id = 2, Username = "ahmad", Password = "ahmad123", Role = "Patient" }
            // Add more users as needed
        };
        /// <summary>
        /// Dependency Injection is used here
        /// </summary>
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDTO.UserName) ||
                     string.IsNullOrEmpty(loginDTO.Password))
                {
                    return BadRequest("Username and/or Password not specified");
                }

                // Find user by username and password
                var user = _users.Find(u => u.Username == loginDTO.UserName && u.Password == loginDTO.Password);
                if (user != null)
                {
                    // Authentication successful, generate JWT token
                    var token = GenerateJwtToken(user);
                    return Ok(token);
                }
                else
                {
                    // Invalid credentials
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred in generating the token: {ex.Message}");
                return BadRequest("An error occurred in generating the token");
            }
        }

        private string GenerateJwtToken(User user)
        {
            // Use a strong key with sufficient length
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisasecretkey@1234567890123456")); // 32 bytes

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) // Include user's role as a claim
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "ABCXYZ",
                audience: "http://localhost:7221",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
