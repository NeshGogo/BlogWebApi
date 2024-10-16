using Azure.Storage.Blobs;
using Contracts;
using Domain.ConfigurationModels;
using Domain.Entities;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.AiServices;
using Persistence.Caching;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using Services;
using Services.Abstractions;
using System.Text;

namespace BlogWebApi.Extensions;

public static class ServiceExtension
{
    public static void ConfigureSerilog(this IHostBuilder host, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("SystemName", configuration["SystemName"])
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Seq(configuration["Seq:ServerUrl"])
            .WriteTo.Console()
            .WriteTo.File("logs/app.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        host.UseSerilog(Log.Logger);
    }

    public static void ConfigureLoggerService(this IServiceCollection services) => services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContextPool<AppDbContext>(opt =>
        {
            var connectionString = configuration.GetConnectionString("Default");
            opt.UseNpgsql(connectionString);
        });

    public static void ConfigureIdentity(this IServiceCollection services) =>
        services.AddIdentityApiEndpoints<User>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidIssuers = [configuration["jwt:validIssuer"]],
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(configuration["jwt:key"])),
                       ClockSkew = TimeSpan.Zero
                   }
                );

    public static void ConfigureRedisCaching(this IServiceCollection services, IConfiguration configuration) =>
        services.AddStackExchangeRedisCache(opt =>
        {
            string connection = configuration.GetConnectionString("Redis");
            opt.Configuration = connection;
        });

    public static void ConfigureServiceManager(this IServiceCollection services) => services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureCachingService(this IServiceCollection services) => services.AddSingleton<ICachingService, CachingService>();

    public static void ConfigureAzureStorage(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton(p => new BlobServiceClient(configuration["ConnectionStrings:StorageAccount"]));

    public static void ConfigureOpenAI(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<OpenAIConfiguration>(configuration.GetSection(OpenAIConfiguration.SectionName))
                .AddScoped<IGenerativeAI, GenerativeAiService>();

    public static void ConfigureSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(opt =>
        {
            opt.EnableAnnotations();
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog Post API", Version = "v1" });
            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id=JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[]{}
                }
            });
        });

}
