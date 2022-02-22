﻿using FutureNHS.Api.Exceptions;
using System.Net;
using System.Text.Json;

namespace FutureNHS.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var result = string.Empty;

            switch (exception)
            {
                case NotModifiedException notModifiedException:
                    httpStatusCode = HttpStatusCode.NotModified;
                    result = JsonSerializer.Serialize(new { error = notModifiedException.Message });
                    break;
                case PreconditionFailedExeption preconditionFailedException:
                    httpStatusCode = HttpStatusCode.PreconditionFailed;
                    result = JsonSerializer.Serialize(new { error = preconditionFailedException.Message });
                    break;
                case NotFoundException recordNotFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = recordNotFoundException.Message });
                    break;
                case ForbiddenException forbiddenException:
                    httpStatusCode = HttpStatusCode.Forbidden;
                    result = JsonSerializer.Serialize(new { error = forbiddenException.Message });
                    break;
                default:
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
            }

            context.Response.StatusCode = (int)httpStatusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
