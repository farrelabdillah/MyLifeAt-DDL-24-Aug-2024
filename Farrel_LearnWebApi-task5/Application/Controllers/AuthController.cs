using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Persistence.Models;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static Dictionary<string, string> _refreshTokens = new Dictionary<string, string>(); // Simpan refresh token sementara

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint untuk login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Validasi username dan password "admin"
            if (login.Username == "admin" && login.Password == "admin")
            {
                var token = GenerateJwtToken(login.Username);
                var refreshToken = GenerateRefreshToken();
                _refreshTokens[refreshToken] = login.Username; // Simpan refresh token

                return Ok(new TokenResponse
                {
                    Token = token,
                    RefreshToken = refreshToken
                });
            }

            // Jika username atau password salah, kembalikan Unauthorized
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Endpoint untuk refresh token
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenModel refreshTokenModel)
        {
            if (_refreshTokens.TryGetValue(refreshTokenModel.RefreshToken, out var username))
            {
                var token = GenerateJwtToken(username);
                var newRefreshToken = GenerateRefreshToken();

                // Hapus refresh token lama dan simpan yang baru
                _refreshTokens.Remove(refreshTokenModel.RefreshToken);
                _refreshTokens[newRefreshToken] = username;

                return Ok(new TokenResponse
                {
                    Token = token,
                    RefreshToken = newRefreshToken
                });
            }

            return Unauthorized(new { message = "Invalid refresh token" });
        }

        // Fungsi untuk menghasilkan JWT token
        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Fungsi untuk menghasilkan Refresh Token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }

    // Model untuk menerima data login
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // Model untuk menyimpan Token Response
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    // Model untuk menerima data Refresh Token
    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; }
    }
}
