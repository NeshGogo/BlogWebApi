using BlogWebApi.Extensions;
using Contracts;
using Domain.Entities;
using Domain.Repositories;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Repositories;
using Serilog;
using Services;
using Services.Abstractions;
using System.Text;
using Serilog.Sinks.Seq;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// --> Log configuration
Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("SystemName", builder.Configuration["SystemName"])
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
            .WriteTo.Console()            
            .WriteTo.File("logs/app.txt", rollingInterval: RollingInterval.Day)            
            .CreateLogger();

builder.Host.UseSerilog(Log.Logger);
Log.Information("Staring host");
// Add services to the container.

// --> Register the controllers that are in the presentation class library (Presentation layer)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

// --> Register DbContext
builder.Services.AddDbContextPool<AppDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    opt.UseNpgsql(connectionString);
});

// Register Identity
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT authetication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = false,
           ValidateLifetime = true,
           ValidIssuers = [builder.Configuration["jwt:validIssuer"]],
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
           ClockSkew = TimeSpan.Zero
       }
    );
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
// --> Registering the services Manager and Repository Manager
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

// --> Logger service
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddSerilog();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
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

var app = builder.Build();
app.UseSerilogRequestLogging(opt =>
{
    opt.EnrichDiagnosticContext = (context, httpContext) =>
    {
        context.Set("RequestRemoteIpAddress", httpContext.Connection.RemoteIpAddress.ToString());
        context.Set("RequestLocalIpAddress", httpContext.Connection.LocalIpAddress.ToString());
        context.Set("RequestScheme", httpContext.Request.Scheme);
        context.Set("RequestHost", httpContext.Request.Host.ToString());
        context.Set("RequestPath", httpContext.Request.Path);
        context.Set("RequestMethod", httpContext.Request.Method);
        context.Set("RequestResponseStatus", httpContext.Response.StatusCode.ToString());
        context.Set("RequestBrowserUserAgent", httpContext.Request.Headers.UserAgent.ToString());
        context.Set("RequestAuthorizationHeader", httpContext.Request.Headers.Authorization.ToString());
    };
});
app.ConfigureExceptionHandler(app.Services.GetRequiredService<ILogger<object>>());

if (app.Environment.IsProduction())
    app.UseHsts();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

// --> Map Identity endpoints
//app.MapGroup("Account").MapIdentityApi<User>();

app.MapControllers();

// --> Run migrations and seed data.
PreDb.PrePopulation(app.Services);
app.Run();