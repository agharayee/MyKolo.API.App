using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyKolo.API.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; }

        //for a one to many relationship.
        //This is just to hold a list of expenses, savings a particular user
        //parent to child relationship
        public virtual IEnumerable<Expenses> Expenses { get; set; }
        public virtual IEnumerable<Savings> Savings { get; set; }

    }
}
