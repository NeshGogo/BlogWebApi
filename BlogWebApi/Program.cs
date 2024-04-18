using Services;
using Services.Abstractions;
using Domain.Repositories;
using Persistence.Repositories;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["jwt:key"])),
           ClockSkew = TimeSpan.Zero
       }
    );


// --> Registering the services Manager and Repository Manager
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// --> Map Identity endpoints
app.MapGroup("Account").MapIdentityApi<User>();

app.MapControllers();

// --> Run migrations and seed data.
PreDb.PrePopulation(app.Services);
app.Run();
