using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyKolo.API.Dtos
{
    public class CreateExpensesDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
    }
}
