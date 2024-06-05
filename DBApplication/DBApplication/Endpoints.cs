using DBApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service;

namespace Endpoints
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IDatabaseConnectionManager, MySqlConnectionManager>();
            services.AddScoped<WeaponsDAO>();
            services.AddScoped<WeaponService>();

            var connectionString = Configuration.GetConnectionString("ConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string empty or not found");
            }
            services.AddSingleton(connectionString);

            services.AddLogging(configure => configure.AddConsole());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class WeaponRestAPI
    {
        private readonly WeaponService _weaponService;

        public WeaponRestAPI(WeaponService weaponService) => _weaponService = weaponService; 

        public string GetAllWeaponsJson()
        {
            return _weaponService.GetAllWeaponsJson();
        }

        public string GetWeaponByIdJson(int id)
        {
            return _weaponService.GetWeaponByIdJson(id);
        }
    }

    namespace Endpoints.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class WeaponsController : ControllerBase
        {
            private readonly WeaponRestAPI _weaponRestAPI;

            public WeaponsController(WeaponService weaponService)
            {
                _weaponRestAPI = new WeaponRestAPI(weaponService);
            }

            [HttpGet]
            [Route("json")]
            public ActionResult<string> GetAllWeaponsJson()
            {
                try
                {
                    var weaponJson = _weaponRestAPI.GetAllWeaponsJson();
                    return Ok(weaponJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server error: {ex.Message}");
                    return StatusCode(500, $"Server error: {ex.Message}");
                }
            }

            [HttpGet]
            [Route("{id}/json")]
            public ActionResult<string> GetWeaponByIdJson(int id)
            {
                try
                {
                    var weaponJson = _weaponRestAPI.GetWeaponByIdJson(id);
                    return Ok(weaponJson);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Weapon not found: {ex.Message}");
                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server error: {ex.Message}");
                    return StatusCode(500, $"Server error: {ex.Message}");
                }
            }
        }
    }
}