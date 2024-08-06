using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos_System.API.Utils;
using SE170311.Lab3.Enums;
using SE170311.Lab3.Payload.Request.Auth;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Utils;

namespace SE170311.Lab3.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public AuthenticationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var loginAccount = _unitOfWork.AccountRepository
                .Get(filter: account => account.Username.Equals(loginRequest.Username)
                && account.Password.Equals(PasswordUtil.HashPassword(loginRequest.Password)),
                includeProperties: "Role").FirstOrDefault();
            if (loginAccount == null)
            {
                throw new KeyNotFoundException("Invalid username or password");
            }
            else if (loginAccount.Status == AccountStatus.Deactive.ToString())
            {
                throw new BadHttpRequestException("Account is not active");
            }
            var token = JwtUtil.GenerateJwtToken(loginAccount);
            var loginResponse = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = new
                {
                    accessToken = token,
                    user = loginAccount
                }
            };
            return Ok(loginResponse);
        }
    }
}
