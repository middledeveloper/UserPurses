using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserPurses.Models
{
    public class Account
    {
        public int Id { get; set; }
        public Purse Purse { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
