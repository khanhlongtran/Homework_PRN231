using ClientAuthentication;

namespace ClientAuthenticatation.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddTransient<IClientSourceAuthenticationHandler>(serviceProvider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("ClientAuthentication") ?? throw new Exception("Cannot find or connect to SQL Server");
                return new SqlServerClientSourceAuthenticationHandler(connectionString);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
