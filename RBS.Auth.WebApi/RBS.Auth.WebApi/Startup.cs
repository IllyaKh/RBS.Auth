using System;
using System.IO;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RBS.Auth.Common;
using RBS.Auth.Db;
using RBS.Auth.Services.Authenticate;
using RBS.Auth.Services.Hashing;
using RBS.Auth.Services.Interfaces.Authenticate;
using RBS.Auth.Services.Interfaces.Hashing;
using RBS.Auth.Services.Interfaces.Tokens;
using RBS.Auth.Services.Interfaces.Verification;
using RBS.Auth.Services.Tokens;
using RBS.Auth.Services.Verification;

namespace RBS.Auth.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Environment = env;

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables();
        
        if (Environment.IsProduction())
        {
            var bytes = Convert.FromBase64String(System.Environment.GetEnvironmentVariable("AppSettings"));
            builder.AddJsonStream(new MemoryStream(bytes));
        }

        Configuration = builder.Build();
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureDatabase(Configuration);

        services.AddAutoMapper(typeof(Startup));

        ConfigureInternalServices(services);

        services.AddControllers().AddFluentValidation(fv => 
            fv.RegisterValidatorsFromAssemblyContaining<Startup>());

        services.Configure<AuthOptions>(Configuration.GetSection("Auth"));
        services.Configure<ServicesOptions>(Configuration.GetSection("Services"));


        if (!Environment.IsProduction())
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "RBS.Auth.WebApi", Version = "v1"});
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
        IServiceProvider serviceProvider, AuthContext context)
    {
        if (!env.IsDevelopment())
        {
            serviceProvider.MigrateDatabase();
        }

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                opt.RoutePrefix = string.Empty;
            });
            app.UseDeveloperExceptionPage();
        }


        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private void ConfigureInternalServices(IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticateService, AuthenticateService>();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<IVerificationService, VerificationService>();
    }
}