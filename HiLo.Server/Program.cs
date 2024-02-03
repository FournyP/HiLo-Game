using HiLo.Server.Interfaces;
using HiLo.Server.Services;
using Serilog;
using System.Net.WebSockets;

namespace HiLo.Server
{
    public class Program
    {
        /// <summary>
        ///     Entry point of the project.
        ///     Notice the method is returning async task instead of void in order to support async programming.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseUrls("http://localhost:5000");

                builder.Configuration
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

                // configure serilog
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .CreateLogger();

                Log.Information("Starting up...");

                // Create service collection and configure our services
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();

                app.UseWebSockets();


                app.Map("/", async (context) =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();

                        var gameEngine = context.RequestServices.GetService<IGameEngine>();
                        await gameEngine.ConnectPlayer(ws);
                        while (ws.State == WebSocketState.Open) { }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                });

                var gameEngine = app.Services.GetRequiredService<IGameEngine>();
                var gameEngineThread = new Thread(() => gameEngine.StartEngine().Wait());
                gameEngineThread.Start();

                app.Run();

                gameEngine.Dispose();
                gameEngineThread.Join();

                Log.Information("Shutting down...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        ///     Configure the services using ASP.NET CORE built-in Dependency Injection.
        /// </summary>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuration should be singleton as the entire application should use one
            services.AddOptions();

            // Register the services
            services.AddSingleton<IGameEngine, GameEngine>();
        }
    }
}
