using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

public class Startup
{
    // Constructor
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Configuration property
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // MongoDB configuration
        var mongoConfig = Configuration.GetSection("MongoDB");
        var connectionString = mongoConfig.GetValue<string>("ConnectionString");
        var databaseName = mongoConfig.GetValue<string>("DatabaseName");

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Swagger configuration
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
