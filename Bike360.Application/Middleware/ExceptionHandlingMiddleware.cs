using Bike360.Application.Common;
using Bike360.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Bike360.Application.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"[ENDPOINT]: {context.Request.Path}");
                await _next(context);
                _logger.LogInformation($"[ENDPOINT]: {context.Request.Path} [STATUS] {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }


        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorMessages) = exception switch
            {
                NotFoundException notFoundException =>
                    (HttpStatusCode.NotFound, new List<string> { notFoundException.Message }),

                BadRequestException badRequestException =>
                    (HttpStatusCode.BadRequest, new List<string> { badRequestException.Message }),

                UnauthorizedException unauthorizedException =>
                    (HttpStatusCode.Unauthorized, new List<string> { unauthorizedException.Message }),

                ForbiddenException forbiddenException =>
                    (HttpStatusCode.Forbidden, new List<string> { forbiddenException.Message }),

                ValidationException validationException =>
                    (HttpStatusCode.BadRequest, validationException.Errors
                        .SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}"))
                        .ToList()),

                _ => (HttpStatusCode.InternalServerError,
                    new List<string> { "Internal Server Error from the custom middleware." })
            };

            _logger.LogError(
                exception,
                "STATUS: [ExceptionHandlingMiddleware][HandleExceptionAsync] EXCEPTION_OCCURRED | StatusCode: {StatusCode} ({StatusCodeValue}) | ExceptionType: {ExceptionType} | Messages: {ErrorMessages} | Path: {RequestPath}",
                statusCode,
                (int)statusCode,
                exception.GetType().Name,
                string.Join(", ", errorMessages),
                context.Request.Path
            );

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, unable to write error response.");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var apiResponse = new ApiResponse(
                data: null,
                errors: errorMessages,
                statusCode: statusCode
            );

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, options));

        }
    }
}
