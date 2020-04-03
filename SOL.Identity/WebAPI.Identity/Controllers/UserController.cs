using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Domain;
using WebAPI.Identity.Dto;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace WebAPI.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper
            )
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {


                var user = await _userManager.FindByNameAsync(userDto.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = userDto.UserName,
                        Email = userDto.UserName
                    };

                    var result = await _userManager.CreateAsync(
                        user, userDto.Password);

                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName.ToUpper());

                        var token = GenerateJWTToken(appUser).Result;
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Home",
                                new { token, email = user.Email }, Request.Scheme);

                        System.IO.File.WriteAllText("confirmEmailAddress.txt", confirmationEmail);

                    }

                }
                return Unauthorized();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"ERRO { ex.Message }");
            }

        }

        private async Task<string> GenerateJWTToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
