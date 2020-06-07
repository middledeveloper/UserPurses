using System;

namespace UserPurses.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public DateTime Updated { get; set; }
        public string CurrencySource { get; set; }
    }
}
