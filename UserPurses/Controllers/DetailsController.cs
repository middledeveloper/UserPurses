using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using UserPurses.Models;
using UserPurses.ViewModels;
using WebAPIApp.Models;

namespace UserPurses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        PurseContext ctx;

        public DetailsController(PurseContext context)
        {
            ctx = context;
        }

        [HttpGet("{userId}")]
        public ActionResult<List<AccountModel>> Get(int userId)
        {
            //https://localhost:44398/api/details/1

            var user = ctx.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            user.Purse = ctx.Purses.Include(p => p.Accounts).FirstOrDefault(p => p.UserId == userId);
            user.Purse.Accounts.ToList().ForEach(a => a.Currency = ctx.Currencies.FirstOrDefault(c => c.Id == a.CurrencyId));

            var result = new List<AccountModel>();
            user.Purse.Accounts
                .ToList()
                .ForEach(a => result.Add(new AccountModel() { 
                    Title = a.Currency.Title,
                    Amount = Currency.ConvertToStr(a.Amount) 
                }));

            return new JsonResult(result);
        }
    }
}
