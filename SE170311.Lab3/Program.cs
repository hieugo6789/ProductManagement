using Microsoft.AspNetCore.Authentication.JwtBearer;
using SE170311.Lab3.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SE170311.Lab3.Repo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var errorResponse = new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "One or more validation errors occurred",
                Data = new
                {
                    errors
                }
            };

            return new JsonResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "SE170311 - Lab 03 - Product Management API",
        Description = "Document for SE170311 - Lab 03 - Product Management API",
    }
    );
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[] { }
                }
            });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("alloworigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddDatabase();
builder.Services.AddUnitOfWork();
builder.Services.AddJwtValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
