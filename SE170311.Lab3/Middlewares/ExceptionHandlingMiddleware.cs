using Microsoft.AspNetCore.Authorization;
using SE170311.Lab3.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data;
using System.Net;
using SE170311.Lab3.Payload.Response;

namespace SE170311.Lab3.Middlewares
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
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await HandleExceptionAsync(context, new UnauthorizedAccessException("Unauthorized access"));
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleExceptionAsync(context, new Exception("Forbidden access"));
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse { Message = exception.Message };
            switch (exception)
            {
                case Exception ex when ex.Message == "Forbidden access":
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                    _logger.LogInformation(exception.Message);
                    break;
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    _logger.LogInformation(exception.Message);
                    break;
                case BadHttpRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogInformation(exception.Message);
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogInformation(exception.Message);
                    break;
                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    _logger.LogInformation(exception.Message);
                    break;
                case ArgumentNullException:
                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogInformation(exception.Message);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception.ToString());
                    break;
            }

            var result = errorResponse.ToString();
            await context.Response.WriteAsync(result);
        }
    }

    public class AuthorizePolicyAttribute : AuthorizeAttribute
    {
        public AuthorizePolicyAttribute(params RoleEnum[] roleEnums)
        {
            var allowedRolesAsString = roleEnums.Select(GetRoleDescription);
            Roles = string.Join(",", allowedRolesAsString);
        }

        private string GetRoleDescription(RoleEnum roleEnum)
        {
            var fi = roleEnum.GetType().GetField(roleEnum.ToString());
            if (fi != null)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }
            return roleEnum.ToString();
        }
    }
}
