using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Data;
using ProductsApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProductsContext _context;

        public AuthController(ProductsContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/account")]
        public async Task<IActionResult> CreateAccount(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                },
                token = GenerateToken()
            });
        }

        [HttpPost]
        [Route("/auth/login")]
        public async Task<IActionResult> Login(Login login)
        {
            var user = await _context.Users.Where(p => p.Email == login.Email && p.Password == login.Password).FirstOrDefaultAsync();

            if (user == null)
               return Unauthorized();

            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email
                },
                token = GenerateToken()
            });
        }

        private static string GenerateToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
