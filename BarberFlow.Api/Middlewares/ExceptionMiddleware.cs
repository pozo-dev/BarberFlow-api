using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Application.Features.Users.Exceptions;
using BarberFlow.Application.Features.Collaborators.Exceptions;
using BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;
using BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;
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

                ForbiddenAccessException => StatusCodes.Status403Forbidden,

                BranchNotFoundException => StatusCodes.Status404NotFound,

                CollaboratorNotFoundException => StatusCodes.Status404NotFound,

                UserNotFoundException => StatusCodes.Status404NotFound,

                UserAlreadyExistsException => StatusCodes.Status409Conflict,

                DuplicateBranchNameException => StatusCodes.Status409Conflict,

                AppointmentConflictException => StatusCodes.Status409Conflict,

                ClientAppointmentCannotBeCancelledException => StatusCodes.Status409Conflict,

                ClientAppointmentCannotBeRescheduledException => StatusCodes.Status409Conflict,

                ClientAppointmentRescheduleConflictException => StatusCodes.Status409Conflict,

                ClientAppointmentNotFoundException => StatusCodes.Status404NotFound,

                ClientAppointmentNotFoundForRescheduleException => StatusCodes.Status404NotFound,

                UserNotActiveException => StatusCodes.Status401Unauthorized,

                InvalidCredentialsException => StatusCodes.Status401Unauthorized,

                InvalidRefreshTokenException => StatusCodes.Status401Unauthorized,

                UnauthorizedException => StatusCodes.Status401Unauthorized,

                RefreshTokenReuseDetectedException => StatusCodes.Status401Unauthorized,

                BarberFlow.Application.Features.BarberShops.Exceptions.BarberShopNotFoundException => StatusCodes.Status404NotFound,

                BarberFlow.Application.Features.Services.Exceptions.ServiceNotFoundException => StatusCodes.Status404NotFound,

                AppointmentNotFoundException => StatusCodes.Status404NotFound,

                CannotCancelOthersAppointmentException => StatusCodes.Status403Forbidden,

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
