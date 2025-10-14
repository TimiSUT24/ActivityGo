using Api.Controllers.Admin;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var problem = CreateProblemDetails(context, ex);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { WriteIndented = true});
            await context.Response.WriteAsync(json);
        }

        private static ProblemDetails CreateProblemDetails(HttpContext context, Exception ex)
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Instance = context.Request.Path
            };

            switch (ex)
            {
                case UnauthorizedAccessException:
                    problem.Title = "Unauthorized";
                    problem.Status = (int)HttpStatusCode.Unauthorized;
                    problem.Detail = ex.Message;
                    break;

                case KeyNotFoundException:
                    problem.Title = "Not Found";
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Detail = ex.Message;
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    problem.Title = "Bad Request";
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    problem.Detail = ex.Message;
                    break;

                case ConflictException:
                    problem.Title = "Conflict";
                    problem.Status = (int)HttpStatusCode.Conflict;
                    problem.Detail = ex.Message;
                    break;

                default:
                    problem.Title = "Internal Server Error";
                    problem.Status = (int)HttpStatusCode.InternalServerError;
                    problem.Detail = "An unexpected error occurred. Please try again later.";
                    break;
            }

            return problem;
        }
    }
}
