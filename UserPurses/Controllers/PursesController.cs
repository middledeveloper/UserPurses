using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserPurses.Models;
using WebAPIApp.Models;

namespace UserPurses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PursesController : ControllerBase
    {
        PurseContext ctx;

        public PursesController(PurseContext context)
        {
            ctx = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await ctx.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            user.Purse = await ctx.Purses.Include(p => p.Accounts).FirstOrDefaultAsync(x => x.UserId == user.Id);

            return new ObjectResult(user);
        }
    }
}
