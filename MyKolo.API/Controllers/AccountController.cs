using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyKolo.API.Dbcontexts;
using MyKolo.API.Dtos;
using MyKolo.API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyKolo.API.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }
        [HttpPost("Register")]
        public IActionResult Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            else if (model == null) return BadRequest();
            else
            {
                try
                {
                    User user = new User
                    {
                        Email = model.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        UserName = model.UserName,
                        Id = Guid.NewGuid().ToString(),
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    var userToReturn = user;
                    return Created("Created Succesfully", new { UserName = user.UserName, Email = user.Email });
                    //return CreatedAtRoute("GetAuthor", new { authorId = authorFromDb.Id }, authorFromDb);
                }
                catch (DbUpdateException ex)
                {
                    return BadRequest($"Email already exist, Please try again. {ex.Message}" );
                }
            }  
        }

        [HttpPost("Login")]
        
        public IActionResult Login([FromBody]LoginDto model)
        {
            //first find the email if it exist in the db
            var foundEmail = _context.Users.FirstOrDefault(user => user.Email.ToLower() == model.Email.ToLower());
            if(foundEmail == null)
            {
                return BadRequest("Invalid Credentials");
            }
            else
            {
                if(!BCrypt.Net.BCrypt.Verify(model.Password, foundEmail.Password))
                {

                    return BadRequest("Invalid Credentials");
                }
                else
                {
                    //Assign a jwt to a user
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, foundEmail.Email),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                    
                    var authSiginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(2),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSiginKey, SecurityAlgorithms.HmacSha256Signature)
                        ); ;

                    return Ok(new
                    {
                        message = "Success",
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        ValidTo = token.ValidTo.ToString("yyyy-MM-ddThh:mm:ss")
                    });
                }
            
            }
        }
    }
}
