using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyKolo.API.Dbcontexts;
using MyKolo.API.Dtos;
using MyKolo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyKolo.API.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUsersDto model)
        {
            if(ModelState.IsValid)
            {
                User users = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    Id = Guid.NewGuid().ToString(),
                    
                };
               await _context.Users.AddAsync(users);
               await _context.SaveChangesAsync();
                return Ok(users.Id);

            }else
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<GetUserDto> getUsers = new List<GetUserDto>();
            var users = await _context.Users.ToListAsync();
            if (users.Any())
            {
               
                foreach (var item in users)
                {
                    GetUserDto userDtos =new GetUserDto
                    {
                       UserName = item.UserName,
                       Email = item.Email,
                       CreatedDate = item.CreatedDate,
                    };
                    getUsers.Add(userDtos);
                }
                return Ok(getUsers);
            }
            else
                return Ok("No User Found");
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<GetUserDto>> GetAUser(string userId)
        {
            if(userId == null)
            {
                return Ok();
            }
            else
            {
                //List<decimal> savings = new List<decimal>();
                //List<decimal> expenses = new List<decimal>();
                var foundUser = await _context.Users.Include(u => u.Expenses).Include(u => u.Savings).FirstOrDefaultAsync(u => u.Id == userId);
                if(foundUser == null)
                {
                    return NotFound();
                }
                else
                {
                    //foreach(var item in foundUser)
                    //{
                    //    GetUserDto getUsers = new GetUserDto
                    //    {
                    //        Expenses = foundUse
                    //    };
                    //}
                    GetUserDto getUser = new GetUserDto
                    {
                        UserName = foundUser.UserName,
                        Email = foundUser.Email,
                        PhoneNumber = foundUser.PhoneNumber,
                        CreatedDate = foundUser.CreatedDate,
  
                    };
                    return Ok(getUser);
                }
            }

        }



       
    }
}
