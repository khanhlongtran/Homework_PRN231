using FrontEnd.Handler;

namespace FrontEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor(); // Để truy cập HttpContext
            builder.Services.AddTransient<TokenHandler>(); // Đăng ký TokenHandler
            builder.Services.AddHttpClient("ApiClient")
                .AddHttpMessageHandler<TokenHandler>(); // Gắn TokenHandler vào HttpClient

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}