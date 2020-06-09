using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UserPurses.Models;
using UserPurses.ViewModels;
using WebAPIApp.Models;

namespace UserPurses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        PurseContext ctx;

        public TransferController(PurseContext context)
        {
            ctx = context;
        }

        [HttpGet("{userId}/{currencyFrom}/{currencyTo}/{amount}/{ratesSource?}")]
        public ActionResult<List<AccountModel>> Get(int userId, string currencyFrom, string currencyTo, decimal amount, string ratesSource = null)
        {
            //https://localhost:44398/api/transfer/1/CHF/SEK/150 & optional /ratesSource

            var ratesActual = CheckRatesUpdateDate();
            if (!ratesActual || ratesSource != null)
            {
                var currencies = Currency.Import(ratesSource == null ? 
                    ctx.Configuration.First().CurrencySource :
                    ratesSource);

                if (currencies == null)
                    return NoContent();

                RefillRates(currencies.ToList());
            }

            var user = ctx.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            user.Purse = ctx.Purses.Include(p => p.Accounts).FirstOrDefault(p => p.UserId == userId);
            user.Purse.Accounts.ToList().ForEach(a => a.Currency = ctx.Currencies.FirstOrDefault(c => c.Id == a.CurrencyId));
            var accountFrom = user.Purse.Accounts.FirstOrDefault(a => a.Currency.Title.ToUpper() == currencyFrom.ToUpper());
            var accountTo = user.Purse.Accounts.FirstOrDefault(a => a.Currency.Title.ToUpper() == currencyTo.ToUpper());

            if (accountFrom == null || accountTo == null)
                return NotFound("Account not found.");

            if (accountFrom.Amount >= amount)
            {
                var baseCurrecySum = amount / accountFrom.Currency.Rate;
                accountFrom.Amount -= amount;

                var targetAccountAmount = baseCurrecySum * accountTo.Currency.Rate;
                accountTo.Amount += targetAccountAmount;

                ctx.SaveChanges();
            }
            else
                return NotFound("Not enough money.");

            var result = new List<AccountModel>();
            user.Purse.Accounts
                .ToList()
                .ForEach(a => result.Add(new AccountModel() {
                    Title = a.Currency.Title,
                    Amount = Currency.ConvertToStr(a.Amount)
                }));

            return new JsonResult(result);
        }

        private bool CheckRatesUpdateDate()
        {
            if (ctx.Configuration.First().Updated == DateTime.Now.Date)
                return true;

            return false;
        }

        private void RefillRates(List<Currency> currencies)
        {
            ctx.Currencies.ToList().ForEach(c => c.Rate = currencies.First(cur => cur.Title == c.Title).Rate);
            ctx.Configuration.First().Updated = DateTime.Now.Date;
            ctx.SaveChanges();
        }
    }
}
