using BlogWebApi.Extensions;
using Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ConfigureOpenTelemetryLoggin();
builder.Host.ConfigureSerilog(builder.Configuration);
Log.Information("Staring host");

builder.Services.ConfigureLoggerService();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRedisCaching(builder.Configuration);
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureCachingService();
builder.Services.AddSerilog();
builder.Services.ConfigureAzureStorage(builder.Configuration);
builder.Services.ConfigureOpenAI(builder.Configuration);
builder.Services.ConfigureOpenTelemetryService(builder.Configuration);

// --> Register the controllers that are in the presentation class library (Presentation layer)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);


builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.IgnoreNullValues = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

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

app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

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