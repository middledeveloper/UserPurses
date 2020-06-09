using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebAPIApp.Models;

namespace UserPurses
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var con = "Server=(localdb)\\mssqllocaldb;Database=pursesdbstore;Trusted_Connection=True;MultipleActiveResultSets=true";
            services.AddDbContext<PurseContext>(options => options.UseSqlServer(con));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // set routes
            });
        }
    }
}
