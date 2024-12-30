using Server.Hubs;
using Server.Services;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .WithOrigins("https://localhost:7111");
                });
            });
            builder.Services.AddScoped<IGameService, GameService>();

            var app = builder.Build();

            app.UseCors();

            app.MapGet("/", () => "Hello World!");

            app.MapHub<ChatHub>("/chat");
            app.MapHub<GameHub>("/game");

            app.Run();
        }
    }
}
