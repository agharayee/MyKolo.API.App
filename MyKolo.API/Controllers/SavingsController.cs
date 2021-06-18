using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("Savings")]
    [Authorize]
    public class SavingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SavingsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> AddSavings(CreateSavingDto model)
        {
            if (ModelState.IsValid)
            {
                Savings savings = new Savings
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = model.Amount,
                    Description = model.Description,
                    UserId = model.UserId
                };
                await _context.Savings.AddAsync(savings);
                await _context.SaveChangesAsync();
                return Ok(savings.Id);

            }
            return Ok();
        }
        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<CreateSavingDto>> GetSavingsForAUser(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            else
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound($"No User found");
                }
                else
                {
                    List<GetSavingDtoForAUser> expenses = new List<GetSavingDtoForAUser>();
                    var userSavings = _context.Savings.Where(s => s.UserId == userId);
                    if (!userSavings.Any())
                    {
                        return Ok($"{user.UserName} You have not started saving");
                    }
                    else
                    {
                        foreach(var item in userSavings)
                        {

                            expenses.Add(new GetSavingDtoForAUser
                            {
                                Id = item.Id,
                                Amount = item.Amount,
                                Description = item.Description,
                                CreatedDateTime = item.CreatedDate
                            }) ;
                        }
                            return Ok(expenses);           
                    }
                }

            }
        }

        [HttpPut]
        public IActionResult UpdateExpensesByUser([FromQuery] string userId, [FromBody] List<UpdateExpensesDto> expensesToUpdate)
        {

            User attemptingUser = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (attemptingUser != null)
            {
                List<Expenses> oldExpenses = _context.Expenses.Where(y => y.UserId == attemptingUser.Id).ToList();
                foreach (var expense in expensesToUpdate)
                {
                    Expenses dbExpense = oldExpenses.FirstOrDefault(c => c.Id == expense.Id);
                    dbExpense.Description = expense.Description;
                    dbExpense.Amount = expense.Amount;

                }
                _context.SaveChanges();
                return NoContent();
            }

            else
            {
                return Unauthorized();
            }
        }

        [HttpDelete]
        public IActionResult DeleteUserExpenses([FromQuery] string userId, [FromBody] List<string> expensesIdToDelete)
        {
            List<Expenses> userExpenses = new List<Expenses>();
            userExpenses = _context.Expenses.Where(c => c.UserId == userId && expensesIdToDelete.Contains(c.Id)).ToList();
            if (userExpenses != null)
            {
                _context.Expenses.RemoveRange(userExpenses);
                _context.SaveChanges();
            }
            return NoContent();
        }
    }
}
