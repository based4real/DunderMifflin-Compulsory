﻿using Microsoft.AspNetCore.Diagnostics;

using Service.Exceptions;

namespace API.ExceptionHandler;

public class ExceptionToProblemDetailsHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "An error occurred",
                Detail = exception.Message,
                Type = exception.GetType().Name,
            },
            Exception = exception
        });
    }
}