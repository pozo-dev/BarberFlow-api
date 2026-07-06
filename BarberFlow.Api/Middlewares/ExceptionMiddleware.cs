using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Application.Features.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BarberFlow.Api.Middlewares
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,

                UserNotFoundException => StatusCodes.Status404NotFound,

                UserAlreadyExistsException => StatusCodes.Status409Conflict,

                UserNotActiveException => StatusCodes.Status401Unauthorized,

                InvalidCredentialsException => StatusCodes.Status401Unauthorized,

                InvalidRefreshTokenException => StatusCodes.Status401Unauthorized,

                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = exception.Message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(problem));
        }
    }
}