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
    [Route("Expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> AddExpenses(CreateExpensesDto model)
        {
            if(ModelState.IsValid)
            {
                Expenses expenses = new Expenses
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = model.UserId,
                    Amount = model.Amount,
                    Description = model.Description,
                };
                await _context.Expenses.AddAsync(expenses);
                await _context.SaveChangesAsync();
                return Ok(expenses.Id);
            }else
            return Ok();
        }
    }
}
