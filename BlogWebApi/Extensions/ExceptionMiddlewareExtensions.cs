using Domain.ErrorModel;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace BlogWebApi.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILogger<object> logger) 
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    {
                        logger.LogError($"--> Something went wrong: {contextFeature.Error}");
                        
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            BadRequestException =>  StatusCodes.Status400BadRequest,
                            NotFoundException => StatusCodes.Status404NotFound,
                            _ => StatusCodes.Status500InternalServerError,
                        };

                        var body = new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error switch
                            {
                                BadRequestException => contextFeature.Error.Message,
                                NotFoundException => contextFeature.Error.Message,
                                _ => "Internal server error",
                            },
                        };
                        await context.Response.WriteAsync(body.ToString());
                    }
                });
            });
        }
    }
}
