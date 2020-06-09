using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using UserPurses.Models;
using WebAPIApp.Models;

namespace MvcMovie.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PurseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<PurseContext>>()))
            {
                if (context.Users.Any())
                {
                    return;
                }

                string defaultSource = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
                var currencies = Currency.Import(defaultSource);
                if (currencies != null)
                {
                    currencies.ToList().ForEach(x => context.Currencies.Add(x));

                    var config = new Configuration()
                    {
                        CurrencySource = defaultSource,
                        Updated = DateTime.Now.Date
                    };

                    context.Configuration.Add(config);
                    context.SaveChanges();
                }

                var user1 = new User { Name = "Tom", Purse = new Purse() };
                var user2 = new User { Name = "Ray", Purse = new Purse() };
                context.Users.AddRange(user1, user2);
                context.SaveChanges();

                FillPurse(user1.Purse, currencies.ToList(), context);
                FillPurse(user2.Purse, currencies.ToList(), context);

                context.SaveChanges();
            }
        }

        private static void FillPurse(Purse purse, List<Currency> currencies, PurseContext ctx)
        {
            purse.Accounts = new List<Account>();
            var random = new Random();

            for (var i = 0; i < 3; i++)
            {
                ctx.Accounts.Add(new Account() {
                    Currency = currencies[random.Next(currencies.Count)],
                    Amount = random.Next(1000, 30000),
                    Purse = purse
                });
            }
        }
    }
}