using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyKolo.API.Controllers
{
    public class GetUserDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; }
        public List<decimal> Expenses { get; set; }
        public List<decimal> Savings { get; set; }
    }
}
