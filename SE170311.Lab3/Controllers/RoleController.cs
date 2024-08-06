using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using SE170311.Lab3.Middlewares;
using SE170311.Lab3.Payload.Request.Accounts;
using SE170311.Lab3.Payload.Request.Roles;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Repo.Models;
using System.Data;

namespace SE170311.Lab3.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RoleController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult GetAllRoles(
            [FromQuery(Name = "role-status")] RoleStatus? roleStatus,
            [FromQuery(Name = "page-index")] int pageIndex = 1,
            [FromQuery(Name = "page-size")] int pageSize = 10
            )
        {
            var roles = _unitOfWork.RoleRepository.Get(
                    filter: c => (!roleStatus.HasValue ||
                    (roleStatus.HasValue && c.Status == roleStatus.ToString())));
            var totalItems = roles.Count();
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
                    data = roles.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult GetRoleById(Guid id)
        {
            var role = _unitOfWork.RoleRepository.GetByID(id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role ID " + id + " does not exist");
            }
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = role
            };
            return Ok(response);
        }

        [HttpPost]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult CreateNewRole(CreateNewRoleRequest createNewRoleRequest)
        {
            Guid roleId = Guid.NewGuid();
            var newRole = new Role
            {
                Id = roleId,
                Name = createNewRoleRequest.Name,
                Status = RoleStatus.Active.ToString()
            };
            _unitOfWork.RoleRepository.Insert(newRole);
            _unitOfWork.Save();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Create new role successfully",
                StatusCode = 200,
                Data = newRole
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult DeleteRole(Guid id)
        {
            var roleExist = _unitOfWork.ProductRepository.GetByID(id);
            if (roleExist == null)
            {
                throw new KeyNotFoundException("Role ID " + id + " does not exist");
            }

            roleExist.Status = RoleStatus.Deactive.ToString();
            _unitOfWork.Save();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Deactivate role successfully",
                StatusCode = 200
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult UpdateRole(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            var updatedRole = _unitOfWork.RoleRepository.GetByID(id);
            if (updatedRole == null)
            {
                throw new KeyNotFoundException("Role ID " + id + " does not exist");
            }

            updatedRole.Name = string.IsNullOrWhiteSpace(updateAccountRequest.Name) ? updatedRole.Name : updateAccountRequest.Name;
            updatedRole.Status = string.IsNullOrWhiteSpace(updateAccountRequest.Status.ToString()) ? updatedRole.Status.ToString() : updateAccountRequest.Status.ToString();

            _unitOfWork.RoleRepository.Update(updatedRole);
            _unitOfWork.Save();

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Update role successfully",
                StatusCode = 200,
                Data = updatedRole
            };
            return Ok(response);
        }
    }

}
