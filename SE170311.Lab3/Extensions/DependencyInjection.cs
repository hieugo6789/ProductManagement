using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SE170311.Lab3.Constants;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Repo.Models;
using System.Collections;
using System.Text;

namespace SE170311.Lab3.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<Lab3Context>(options => options.UseSqlServer(GetConnectionString()));
            return services;
        }

        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
             .AddEnvironmentVariables(prefix: JwtConstant.JwtEnvironment)
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
            var strConn = config["ConnectionStrings:DB"];

            return strConn;
        }

        public static IServiceCollection AddJwtValidation(this IServiceCollection services)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .AddEnvironmentVariables(prefix: JwtConstant.JwtEnvironment)
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();

            // Debug: Print all environment variables
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine($"Key: {de.Key}, Value: {de.Value}");
            }

            var secretKey = configuration["JwtConstant:" + JwtConstant.SecretKey];
            var issuer = configuration["JwtConstant:" + JwtConstant.Issuer];

            // Debug: Print configuration values
            Console.WriteLine($"SecretKey: {secretKey}");
            Console.WriteLine($"Issuer: {issuer}");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            return services;
        }
    }
}
