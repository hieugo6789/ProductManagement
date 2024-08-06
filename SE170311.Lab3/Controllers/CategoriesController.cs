using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using SE170311.Lab3.Middlewares;
using SE170311.Lab3.Payload.Request.Categories;
using SE170311.Lab3.Payload.Response.Categories;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Repo.Models;
using System.Linq.Expressions;

namespace SE170311.Lab3.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff)]
        public IActionResult GetAllCategories([FromQuery] GetCategoryRequest getCategoryRequest)
        {
            var categories = _unitOfWork.CategoryRepository.Get(
                    filter: c => (!getCategoryRequest.categoryStatus.HasValue ||
                    (getCategoryRequest.categoryStatus.HasValue && c.Status == getCategoryRequest.categoryStatus.ToString())));
            int pageIndex = getCategoryRequest.pageIndex ?? 1;
            int pageSize = getCategoryRequest.pageSize ?? 10;
            var totalItems = categories.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = new GetCategoryResponse
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    First = pageIndex == 1,
                    Last = pageIndex == totalPages,
                    data = categories.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff)]
        public IActionResult GetCategoryById(Guid id)
        {
            var category = _unitOfWork.CategoryRepository.GetByID(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category ID " + id + " does not exist");
            }
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = category
            };
            return Ok(response);
        }

        [HttpPost]
        [AuthorizePolicy(RoleEnum.Admin)]
        public async Task<IActionResult> CreateNewCategory(CreateNewCategoryRequest createNewCategoryRequest)
        {
            Guid categoryId = Guid.NewGuid();
            var newCategory = new Category
            {
                Id = categoryId,
                Name = createNewCategoryRequest.Name,
                Status = CategoryStatus.Active.ToString()
            };
            _unitOfWork.CategoryRepository.Insert(newCategory);
            await _unitOfWork.SaveAsync();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Create new category successfully",
                StatusCode = 200,
                Data = newCategory
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult DeleteCategory(Guid id)
        {
            var categoryExist = _unitOfWork.CategoryRepository.GetByID(id);
            if (categoryExist == null)
            {
                throw new KeyNotFoundException("Category ID " + id + " does not exist");
            }

            categoryExist.Status = CategoryStatus.Deactive.ToString();
            _unitOfWork.Save();
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Deactivate category successfully",
                StatusCode = 200
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        [AuthorizePolicy(RoleEnum.Admin)]
        public IActionResult UpdateCategory(Guid id, UpdateCategoryRequest updateCategoryRequest)
        {
            var updatedCategory = _unitOfWork.CategoryRepository.GetByID(id);
            if (updatedCategory == null)
            {
                throw new KeyNotFoundException("Category ID " + id + " does not exist");
            }

            updatedCategory.Name = !string.IsNullOrWhiteSpace(updateCategoryRequest.Name) ? updateCategoryRequest.Name : updatedCategory.Name;
            updatedCategory.Status = updateCategoryRequest.Status.HasValue ? updateCategoryRequest.Status.ToString() : updatedCategory.Status.ToString();

            _unitOfWork.CategoryRepository.Update(updatedCategory);
            _unitOfWork.Save();

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Update category successfully",
                StatusCode = 200,
                Data = updatedCategory
            };
            return Ok(response);
        }
    }
}
