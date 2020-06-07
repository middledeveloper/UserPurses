using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserPurses.Models
{
    public class Purse
    {
        public int Id { get; set; }
        public User User { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
