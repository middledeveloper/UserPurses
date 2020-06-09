using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UserPurses.Models;
using UserPurses.ViewModels;
using WebAPIApp.Models;

namespace UserPurses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefillController : ControllerBase
    {
        PurseContext ctx;

        public RefillController(PurseContext context)
        {
            ctx = context;
        }

        [HttpGet("{userId}/{currency}/{amount}")]
        public ActionResult<AccountModel> Get(int userId, string currency, decimal amount)
        {
            //https://localhost:44398/api/refill/1/HUF/100

            var user = ctx.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            user.Purse = ctx.Purses.Include(p => p.Accounts).FirstOrDefault(p => p.UserId == userId);
            user.Purse.Accounts.ToList().ForEach(a => a.Currency = ctx.Currencies.FirstOrDefault(c => c.Id == a.CurrencyId));
            var account = user.Purse.Accounts.FirstOrDefault(a => a.Currency.Title == currency.ToUpper());

            if (account == null)
                return NotFound("Account not found.");

            account.Amount += amount;
            ctx.SaveChanges();

            var result = new AccountModel() { 
                Title = account.Currency.Title,
                Amount = Currency.ConvertToStr(account.Amount) 
            };

            return new JsonResult(result);
        }
    }
}
