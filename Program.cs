using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

// Configure the web app to use Startup.cs
var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build();

// Run the web app
host.Run();
