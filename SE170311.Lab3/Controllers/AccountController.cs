using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using SE170311.Lab3.Middlewares;
using SE170311.Lab3.Payload.Request.Accounts;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Repo.Models;
using SE170311.Lab3.Utils;

namespace SE170311.Lab3.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult GetAllAccounts(
            [FromQuery(Name = "account-status")] AccountStatus? accountStatus,
            [FromQuery(Name = "page-index")] int pageIndex = 1,
            [FromQuery(Name = "page-size")] int pageSize = 10
            )
        {
            var accounts = _unitOfWork.AccountRepository.Get(
                    filter: c => (!accountStatus.HasValue ||
                    (accountStatus.HasValue && c.Status == accountStatus.ToString())));
            var totalItems = accounts.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    First = pageIndex == 1,
                    Last = pageIndex == totalPages,
                    data = accounts.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff, RoleEnum.Customer)]
        public IActionResult GetAccountById(Guid id)
        {
            var account = _unitOfWork.AccountRepository.GetByID(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account ID " + id + " does not exist");
            }
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = account
            };
            return Ok(response);
        }

        [HttpPost]
        public IActionResult CreateNewAccount(CreateNewAccountRequest createNewAccountRequest)
        {
            var roleExist = _unitOfWork.RoleRepository.GetByID(createNewAccountRequest.RoleId);
            if (roleExist == null)
            {
                throw new KeyNotFoundException("Role ID " + createNewAccountRequest.RoleId + " does not exist");
            }
            else if (roleExist.Status == RoleStatus.Deactive.ToString())
            {
                throw new BadHttpRequestException("Role ID " + createNewAccountRequest.RoleId + " is not active");
            }

            var isUsernameExisted = _unitOfWork.AccountRepository
                .Get(filter: a => a.Username == createNewAccountRequest.Username)
                .FirstOrDefault() != null;
            if (isUsernameExisted)
            {
                throw new BadHttpRequestException("Username " + createNewAccountRequest.Username + " has already existed");
            }

            Guid accountId = Guid.NewGuid();

            var newAccount = new Account
            {
                Id = accountId,
                Name = createNewAccountRequest.Name,
                Password = PasswordUtil.HashPassword(createNewAccountRequest.Password),
                Username = createNewAccountRequest.Username,
                RoleId = createNewAccountRequest.RoleId,
                Status = AccountStatus.Active.ToString()
            };
            _unitOfWork.AccountRepository.Insert(newAccount);
            _unitOfWork.Save();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Create new account successfully",
                StatusCode = 200,
                Data = newAccount
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult DeleteAccount(Guid id)
        {
            var accountExist = _unitOfWork.ProductRepository.GetByID(id);
            if (accountExist == null)
            {
                throw new KeyNotFoundException("Account ID " + id + " does not exist");
            }

            accountExist.Status = AccountStatus.Deactive.ToString();
            _unitOfWork.Save();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Deactivate account successfully",
                StatusCode = 200
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            var updatedAccount = _unitOfWork.AccountRepository.GetByID(id);
            if (updatedAccount == null)
            {
                throw new KeyNotFoundException("Account ID " + id + " does not exist");
            }

            if (!string.IsNullOrWhiteSpace(updateAccountRequest.Name))
            {
                var isUsernameExisted = _unitOfWork.AccountRepository
                    .Get(filter: a => a.Username == updateAccountRequest.Username)
                    .FirstOrDefault() != null;
                if (isUsernameExisted)
                {
                    throw new BadHttpRequestException("Username " + updateAccountRequest.Username + " has already existed");
                }
                updatedAccount.Username = updateAccountRequest.Username;
            }

            updatedAccount.Name = string.IsNullOrWhiteSpace(updateAccountRequest.Name) ? updatedAccount.Name : updateAccountRequest.Name;
            updatedAccount.Password = string.IsNullOrWhiteSpace(updateAccountRequest.Password) ? updatedAccount.Password : updateAccountRequest.Password;
            updatedAccount.Status = string.IsNullOrWhiteSpace(updateAccountRequest.Status.ToString()) ? updatedAccount.Status.ToString() : updateAccountRequest.Status.ToString();

            _unitOfWork.AccountRepository.Update(updatedAccount);
            _unitOfWork.Save();

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Update account successfully",
                StatusCode = 200,
                Data = updatedAccount
            };
            return Ok(response);
        }
    }

}
