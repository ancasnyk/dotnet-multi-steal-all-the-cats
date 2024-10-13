using CatsStealer.WebApi.Extension;
using CatStealer.Application;
using CatStealer.Application.Services;
using Microsoft.EntityFrameworkCore;
using CatStealer.Infrastructure.Data;
using CatStealer.Core.Interfaces.Repositories;
using CatStealer.Infrastructure.Repositories;
using CatStealer.Core.Configuration;
using CatStealer.Infrastructure.Services;

namespace CatsStealer.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Host
                .ConfigureAppConfiguration((hosting, configBuilder) => configBuilder.RegisterConfiguration(hosting));

            builder.Services.Configure<CatsApiSettings>(builder.Configuration.GetSection(nameof(CatsApiSettings)));
            builder.Logging.ClearProviders();

            builder.Logging.AddConsole();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionString"], b => b.MigrationsAssembly("CatStealer.Infrastructure"));
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddScoped<ICatRepository, CatRepository>();
            builder.Services.AddScoped<ICatsStealerService, CatsStealerService>();

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ICatApiClient, CatApiClient>();

            var app = builder.Build();

            SetupDb(app);

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            if (app.Environment.IsDevelopment() == false)
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void SetupDb(IHost app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
            }
        }
    }
}
