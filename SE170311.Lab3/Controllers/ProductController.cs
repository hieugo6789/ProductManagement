using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SE170311.Lab3.Enums;
using SE170311.Lab3.Middlewares;
using SE170311.Lab3.Payload.Request.Product;
using SE170311.Lab3.Payload.Response.Products;
using SE170311.Lab3.Payload.Response;
using SE170311.Lab3.Repo.Implement;
using SE170311.Lab3.Repo.Models;
using SE170311.Lab3.Utils;

using System.Linq.Expressions;

namespace SE170311.Lab3.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Sort: ProductId = 0 | ProductName = 1 | CategoryId = 2 | UnitsInStock = 3 | UnitPrice = 4 | Status = 5. Example: order-fields = 4:asc,3:desc
        /// </summary>
        [HttpGet]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff, RoleEnum.Customer)]
        public IActionResult GetAllProducts([FromQuery] GetProductResquest getProductResquest)
        {
            if (getProductResquest.categoryId != null && getProductResquest.categoryId.HasValue)
            {
                var category = _unitOfWork.CategoryRepository.GetByID(getProductResquest.categoryId);
                if (category == null)
                {
                    throw new KeyNotFoundException("Category ID " + getProductResquest.categoryId + " does not exist");
                }
            }
            var products = _unitOfWork.ProductRepository.Get(filter: p =>
                // query with search name
                (string.IsNullOrEmpty(getProductResquest.searchName) ||
                (!string.IsNullOrEmpty(getProductResquest.searchName) && p.ProductName.ToLower().Contains(getProductResquest.searchName.Trim().ToLower())))

                // query with categoryId
                && (!getProductResquest.categoryId.HasValue || (getProductResquest.categoryId.HasValue && p.CategoryId == getProductResquest.categoryId))

                // query with min, max unit price
                && p.UnitPrice >= getProductResquest.minPrice
                && (!getProductResquest.maxPrice.HasValue || (getProductResquest.maxPrice.HasValue && p.UnitPrice <= getProductResquest.maxPrice))

                // query with produt status
                && (!getProductResquest.productStatus.HasValue ||
                (getProductResquest.productStatus.HasValue && p.Status == getProductResquest.productStatus.ToString()))

                , orderBy: string.IsNullOrEmpty(getProductResquest.orderFields) ? null :
                    query => query.OrderByMultipleFields(ParseOrderByFields(getProductResquest.orderFields)));

            int pageIndex = getProductResquest.pageIndex ?? 1;
            int pageSize = getProductResquest.pageSize ?? 50;
            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = new GetProductResponse
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    First = pageIndex == 1,
                    Last = pageIndex == totalPages,
                    data = products.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                }
            };
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff, RoleEnum.Customer)]
        public IActionResult GetProductById(Guid id)
        {
            var product = _unitOfWork.ProductRepository.GetByID(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product ID " + id + " does not exist");
            }

            var categoryOfProduct = _unitOfWork.CategoryRepository.GetByID(product.CategoryId);

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "",
                StatusCode = 200,
                Data = new
                {
                    product.ProductId,
                    product.ProductName,
                    product.CategoryId,
                    CategoryName = categoryOfProduct.Name,
                    product.UnitsInStock,
                    product.UnitPrice,
                    product.ImageUrl,
                    product.Status,
                }
            };
            return Ok(response);
        }

        [HttpPost]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff)]
        public async Task<IActionResult> CreateNewProduct([FromForm] CreateNewProductRequest createNewProductRequest)
        {
            var categoryExist = _unitOfWork.CategoryRepository.GetByID(createNewProductRequest.CategoryId);
            if (categoryExist == null)
            {
                throw new KeyNotFoundException("Category ID " + createNewProductRequest.CategoryId + " does not exist");
            }
            else if (categoryExist.Status == CategoryStatus.Deactive.ToString())
            {
                throw new BadHttpRequestException("Category ID " + createNewProductRequest.CategoryId + " is not active");
            }

            var newProduct = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = createNewProductRequest.ProductName,
                UnitPrice = createNewProductRequest.UnitPrice,
                UnitsInStock = createNewProductRequest.UnitsInStock,
                CategoryId = createNewProductRequest.CategoryId,
                Status = ProductStatus.Active.ToString()
            };

            if (createNewProductRequest.ImageFile != null && createNewProductRequest.ImageFile.Length > 0)
            {
                var imageFileName = Guid.NewGuid().ToString();
                newProduct.ImageFileName = imageFileName;
                newProduct.ImageUrl = await UploadImageToFirebase(createNewProductRequest.ImageFile, imageFileName);
            }

            _unitOfWork.ProductRepository.Insert(newProduct);
            await _unitOfWork.SaveAsync();

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Create new product successfully",
                StatusCode = 200,
                Data = newProduct
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var productExist = _unitOfWork.ProductRepository.GetByID(id);
                if (productExist == null)
                {
                    throw new KeyNotFoundException("Product ID " + id + " does not exist");
                }
                productExist.Status = ProductStatus.Deactive.ToString();
                await _unitOfWork.SaveAsync();
                var response = new BasicResponse
                {
                    IsSuccess = true,
                    Message = "Deactivate product successfully",
                    StatusCode = 200
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpPut("{id}")]
        [AuthorizePolicy(RoleEnum.Admin, RoleEnum.Staff)]

        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductRequest updateProductRequest)
        {
            var updatedProduct = _unitOfWork.ProductRepository.GetByID(id);
            if (updatedProduct == null)
            {
                throw new KeyNotFoundException("Product ID " + id + " does not exist");
            }


            if (updateProductRequest.CategoryId.HasValue)
            {
                var categoryExist = _unitOfWork.CategoryRepository.GetByID(updateProductRequest.CategoryId);
                if (categoryExist == null)
                {
                    throw new KeyNotFoundException("Category ID " + updateProductRequest.CategoryId + " does not exist");
                }
                else if (categoryExist.Status == CategoryStatus.Deactive.ToString())
                {
                    throw new BadHttpRequestException("Category ID " + updateProductRequest.CategoryId + " is not active");
                }
                updatedProduct.CategoryId = (Guid)updateProductRequest.CategoryId;
            }
            if (!string.IsNullOrWhiteSpace(updateProductRequest.ProductName)) { updatedProduct.ProductName = updateProductRequest.ProductName; }
            if (updateProductRequest.UnitPrice.HasValue) { updatedProduct.UnitPrice = (decimal)updateProductRequest.UnitPrice; }
            if (updateProductRequest.UnitsInStock.HasValue) { updatedProduct.UnitsInStock = (int)updateProductRequest.UnitsInStock; }
            if (updateProductRequest.ProductStatus.HasValue) { updatedProduct.Status = updateProductRequest.ProductStatus.ToString(); }

            if (updateProductRequest.ImageFile != null && updateProductRequest.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(updatedProduct.ImageFileName) && !string.IsNullOrEmpty(updatedProduct.ImageUrl))
                {
                    await DeleteImageFromFirebase(updatedProduct.ImageFileName);
                }
                var imageFileName = Guid.NewGuid().ToString();
                updatedProduct.ImageFileName = imageFileName;
                updatedProduct.ImageUrl = await UploadImageToFirebase(updateProductRequest.ImageFile, imageFileName);
            }

            _unitOfWork.ProductRepository.Update(updatedProduct);
            await _unitOfWork.SaveAsync();

            var response = new BasicResponse
            {
                IsSuccess = true,
                Message = "Update product successfully",
                StatusCode = 200,
                Data = updatedProduct
            };
            return Ok(response);
        }

        private List<(string field, bool isDescending)> ParseOrderByFields(string orderByFields)
        {
            var fields = orderByFields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var orderFieldsList = new List<(string field, bool isDescending)>();

            foreach (var field in fields)
            {
                var parts = field.Split(':');
                var fieldName = CheckProductSortedField(parts[0]);
                var isDescending = parts.Length > 1 && parts[1].ToLower() == "desc";
                orderFieldsList.Add((fieldName, isDescending));
            }

            return orderFieldsList;
        }

        private string CheckProductSortedField(string fieldCode)
        {
            if (string.IsNullOrEmpty(fieldCode) || !int.TryParse(fieldCode, out int fieldNumber))
            {
                throw new Exception("Invalid sorted field");
            }
            switch (fieldNumber)
            {
                case (int)ProductSortedField.ProductId:
                    return ProductSortedField.ProductId.ToString();
                case (int)ProductSortedField.ProductName:
                    return ProductSortedField.ProductName.ToString();
                case (int)ProductSortedField.CategoryId:
                    return ProductSortedField.CategoryId.ToString();
                case (int)ProductSortedField.UnitsInStock:
                    return ProductSortedField.UnitsInStock.ToString();
                case (int)ProductSortedField.UnitPrice:
                    return ProductSortedField.UnitPrice.ToString();
                default:
                    throw new Exception("Invalid sorted field. Not exist this number of sorted field");
            }
        }

        private async Task<string> UploadImageToFirebase(IFormFile file, string fileName)
        {
            try
            {
                var firebase = new FirebaseStorage("prn231-ea891.appspot.com")
                    .Child("lab01")
                    .Child(fileName);

                using (var stream = file.OpenReadStream())
                {
                    var uploadTask = await firebase.PutAsync(stream);
                    return uploadTask;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload file: " + ex.Message);
            }
        }

        private async Task DeleteImageFromFirebase(string fileName)
        {
            try
            {
                var firebase = new FirebaseStorage("prn231-ea891.appspot.com")
                    .Child("lab01")
                    .Child(fileName);
                await firebase.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload file: " + ex.Message);
            }
        }
    }
}
