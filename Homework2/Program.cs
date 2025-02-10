using Authorization;
using ClientAuthentication;
using Homework2.AuthenticationHandler;
using Homework2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Homework2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            IClientSourceAuthenticationHandler clientSourceAuthenticationHandler = new RemoteAuthenticationHandler(builder.Configuration["AuthenticationService"] ?? throw new Exception("Cannot authenticate request"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication("X-Client-Source")
               .AddXClientSource(options =>
               {
                   options.ClientValidator = (clientSourceValue, tokenValue, principal) => clientSourceAuthenticationHandler.Validate(clientSourceValue);
                   options.IssuerSigningKey = builder.Configuration["IssuerSigningKey"] ?? "";
               });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "wwwroot")
            ),
                RequestPath = "" // Important: This should be an empty string if you want to access files directly from wwwroot
            });
            app.MapControllers();

            app.Run();
        }
    }
}
