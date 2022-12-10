
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.DTOs.Product;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.DTOs;
using AdsMarketSharing.DTOs.Payment;
using AdsMarketSharing.DTOs.Review;
using AdsMarketSharing.Models;
using AdsMarketSharing.Services.Payment;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.SignalR;
using AdsMarketSharing.Hubs;

namespace AdsMarketSharing.Controllers
{
    //[Authorize]
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IHubContext<NotifyHub> _notifyContext;
        private readonly SQLExpressContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(SQLExpressContext dbContext, IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService, IHubContext<NotifyHub> notifyContext)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _notifyContext = notifyContext;
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetProductList([FromQuery]FilterProductRequestDTO filterProductRequestDTO)
        {
            try
            {
                var productLstQuery = _dbContext.Products
                    .AsNoTracking()
                    .OrderByDescending(p => p.Orders.Count)
                    .OrderBy(p => p.Name)
                    .Include(p => p.Orders)
                    .Include(p => p.UserPage)
                    .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                    .Include(p => p.ProductClassifies)
                        .ThenInclude(pc => pc.ProductClassifyTypes)
                    .Include(p => p.Attachments)
                        .ThenInclude(p => p.Attachment)
                    .Where(p => !p.HasDeleted && p.HasAccepted)
                    .AsQueryable();

                int productMax = countfilterProductList(_dbContext.Products, filterProductRequestDTO);


                productLstQuery = filterProductList(productLstQuery,filterProductRequestDTO);

                if(!productLstQuery.Any())
                {    
                    return NotFound("Not found the matches");   
                }

                var result = productLstQuery
                    .ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider)
                    .ToList();

                return Ok(new GetProductListWithCount()
                {
                    ProductList = result,
                    Amount = productMax
                });
            }
            catch (System.Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductItem(int id)
        {
            var product = _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Where(p => p.Id == id)
                .Include(p => p.UserPage)
                    .ThenInclude(p => p.PageAvatar)
                .Include(p => p.ProductCategories)
                    .ThenInclude(p => p.Category)
                .Include(p => p.ProductClassifies)
                    .ThenInclude(pc => pc.ProductClassifyTypes)
                .Include(p => p.Attachments)
                    .ThenInclude(p => p.Attachment);

            if(product is null)
            {
                return NotFound("Not Matches");
            }

            var result = await product.ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return Ok(result.ElementAtOrDefault(0));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProductList([FromQuery]FilterProductRequestDTO filter)
        {
            // 1. Get User Identity
            string accountIdStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(accountIdStr, out int accountId);

            var userPage = await _dbContext.UserPages
                .Include(up => up.User)
                .FirstOrDefaultAsync(up => up.User.AccountId == accountId);

            if (userPage.User.AccountId < 0)
            {
                return Forbid("Cannot found your identity");
            }

            // 2. Find the situation of production list
            var productLstQuery = _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Include(p => p.UserPage)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductClassifies)
                    .ThenInclude(pc => pc.ProductClassifyTypes)
                .Include(p => p.Attachments)
                    .ThenInclude(p => p.Attachment)
                .Where(p => p.UserPageId == userPage.Id  && !p.HasDeleted && p.HasAccepted)
                .AsQueryable();

            // 3. Filter product list{
            productLstQuery = productLstQuery.Skip((filter.Page - 1) * filter.Take).Take(filter.Take);

            var result = await productLstQuery
                .ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        private async Task<List<AttachmentResponseDTO>> CreateFolderAndSaveImage(List<IFormFile> files) 
        {
            string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;

            var result = new List<AttachmentResponseDTO>();
            List<AttachmentResponseDTO> responseDTOs = null;

            if (files == null) throw new System.Exception("Your files list is null");

            var tasks = new List<Task<ServiceResponse<AttachmentResponseDTO>>>();
            try
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        
                        var task = _fileStorageService.CreateFolderAndSaveImage(file.FileName, file.OpenReadStream(), usernameStr);
                        tasks.Add(task);
                    }
                }
             
                var response = await Task.WhenAll(tasks.ToArray());
                responseDTOs = response.Select(p => p.Data).ToList();
                return responseDTOs;
            }
            catch (System.Exception)
            {
                return responseDTOs;
            }
        }

        [Authorize]
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentProduct([FromQuery] int amount)
        {
            var query = _dbContext.Products
                .OrderBy(p => p.CreatedAt)
                .Skip(amount)
                .AsQueryable();

            return Ok(new
            {
                recentProducts = query.ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider).ToList()
            });
        }

        [Authorize]
        [HttpGet("out")]
        public async Task<IActionResult> GetOutProducts([FromQuery] int amount)
        {
            var query = _dbContext.Products
                .OrderBy(p => p.CreatedAt)
                .Skip(amount)
                .AsQueryable();

            return Ok(new
            {
                outProducts = query.ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider).ToList()
            });
        }

        [Authorize]
        [HttpGet("uninspected")]
        public async Task<IActionResult> GetUnInspectedAcceptedProduct()
        {
            var productList = _dbContext.Products.OrderBy(p => !p.HasAccepted ? 0 : 1);

            var result = productList.ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider).ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewProduct([FromForm]AddProductRequestDTO request)
        {
            try
            {
                // 1. Convert DTO to Product
                var product = _mapper.Map<Product>(request);
                var classifyTypes = product.ProductClassifies.Select(pc => pc.ProductClassifyTypes).ToArray();

                // 1.1. Add Product to database
                _dbContext.Products.Add(product);

                // 1.2. Add Classify Details to database
                List<ProductClassfiyDetail> classifyDetails = null;
                if (classifyTypes != null)
                {
                    classifyDetails = request.ProductDetails
                        .AsQueryable()
                        .Select(pd => new ProductClassfiyDetail()
                        {
                            Price = pd.Price,
                            Inventory = pd.Inventory,
                            PresentImage = _mapper.Map<Attachment>(pd.PresentImage),
                            ClassifyTypeKey = classifyTypes.ElementAtOrDefault(0).ElementAtOrDefault(pd.ClassifyIndexes.ElementAt(0)),
                            ClassifyTypeValue = classifyTypes.ElementAtOrDefault(1) != null ? classifyTypes.ElementAtOrDefault(1).ElementAtOrDefault(pd.ClassifyIndexes.ElementAt(1)) : null,
                        })
                        .ToList();
                }

                // 1.3. Catch don't have categories
                if (product.ProductCategories is null)
                {
                    return NotFound("Cannot find your categories");
                }

                // 2.   Execute the list of long-running tasks
                var tasks = new List<Task<List<AttachmentResponseDTO>>>();

                // 2.1. Add task of images
                if (request.Files != null)
                {
                    var task = CreateFolderAndSaveImage(request.Files);

                    tasks.Add(task);
                };

                // 2.2. Add task of product type detail images
                if (request.ProductDetails != null)
                {
                    var detailImageLst = request.ProductDetails.Select(d => d.Image).ToList();

                    var task = CreateFolderAndSaveImage(detailImageLst.Where(image => image != null).ToList());

                    tasks.Add(task);
                }


                // 3.  Implement each task of list
                var responses = await Task.WhenAll(tasks).ConfigureAwait(false);

                // 3.1.Handle the product images
                if (responses.ElementAt(0) != null)
                {
                    var attachments = responses.ElementAtOrDefault(0)
                                        .AsQueryable()
                                        .ProjectTo<Attachment>(_mapper.ConfigurationProvider)
                                        .ToList();
                    _dbContext.ProductAttachments.AddRange(attachments.Select(p => new ProductAttachment()
                    {
                        Product = product,
                        Attachment = p
                    }));
                }
                // 3.2.Handle the product classify detail image
                if (classifyDetails != null && responses.ElementAt(1) != null && responses.ElementAt(1).Count > 0)
                {
                    var attachments = responses.ElementAtOrDefault(1)
                                        .AsQueryable()
                                        .ProjectTo<Attachment>(_mapper.ConfigurationProvider)
                                        .ToList();

                    classifyDetails = classifyDetails.Select((p, index) =>
                    {
                        int position = request.ProductDetails.ElementAt(index).ClassifyIndexes[0];
                        var _presentImage = attachments.ElementAtOrDefault(position);
                        p.PresentImage = _presentImage != null ? _presentImage : attachments.ElementAt(0);
                        return p;
                    }).ToList();
                }

                _dbContext.ProductClassfiyDetails.AddRange(classifyDetails);
                _dbContext.SaveChanges();

                return StatusCode(200, new
                {
                    Message = "Added new product",
                    NewProductId = product.Id
                });
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("uploadFiles")]
        public async Task<ActionResult<List<AttachmentResponseDTO>>> UploadImages(IFormFile[] formFiles)
        {
            var attachmentResList = new List<AttachmentResponseDTO>();
            // 1. Create folder on cloudinary 
            string usernameStr = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.Email).Value;
            var createFolderAction = await _fileStorageService.CreateFolder(usernameStr);

            if (createFolderAction.Status != Enum.ResponseStatus.Successed)
            {
                return StatusCode(createFolderAction.StatusCode, attachmentResList);
            }
            // 2. Upload the image to cloudinary
            
            for (int i = 0; i < formFiles.Length; i++)
            {
                // 3. Save as attachment    
                var file = formFiles[i];
                var saveImageAction = await _fileStorageService.SaveImage(file.FileName, file.OpenReadStream(), createFolderAction.Data);
                if (saveImageAction.Status != Enum.ResponseStatus.Successed)
                {
                    return StatusCode(saveImageAction.StatusCode, attachmentResList);
                }
                attachmentResList.Add(saveImageAction.Data);
            }

            return Ok(attachmentResList);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromForm]AddProductRequestDTO request,[FromQuery] int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.Attachments)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductClassifies)
                    .ThenInclude(pc => pc.ProductClassifyTypes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound("Not found matches");

            try
            {
                request.Attachments = await CreateFolderAndSaveImage(request.Files);
                product = _mapper.Map(request, product);

                _dbContext.Update(product);
                await _dbContext.SaveChangesAsync();
                return Ok("Updated");
            }
            catch (System.Exception e)
            {   
                return StatusCode(500, "Server Exception");
            }
        }

        [Authorize]
        [HttpPut("permission")]
        public async Task<IActionResult> InspectPermission([FromQuery] bool isAccepted, [FromQuery] int productId)
        {
            var _upchecked = _dbContext.Products.FirstOrDefault(p => p.Id == productId);
            _upchecked.HasAccepted = isAccepted;
            _dbContext.SaveChanges();

            return Ok(new
            {
                Message = isAccepted ? "The product has been accepted" : "The product has been denied",
                ProductId = _upchecked.Id
            });
        }

        [Authorize]
        [HttpPut("permission/deliver")]
        public async Task<IActionResult> DeliverPermisiion([FromQuery] int status, [FromQuery] int orderId)
        {
            var _upchecked = _dbContext.Orders.FirstOrDefault(p => p.Id == orderId);    
            if (_upchecked == null) {
                return NotFound(new { Message = "Cannot found your order" });
            }
            OrderStatus _orderStatus = (OrderStatus) status;
            _upchecked.OrderStatus = _orderStatus;
            string message = null;
            switch (_orderStatus)
            {
                case OrderStatus.Pending:
                    message = OrderScriptMessage.PendingMessage;
                    break;
                case OrderStatus.Waiting:
                    message = OrderScriptMessage.WaitingMessage;
                    break;
                case OrderStatus.Delivering:
                    message = OrderScriptMessage.DeliveringMessage;
                    break;
                case OrderStatus.Delivered:
                    message = OrderScriptMessage.DeliveredMessage;
                    break;
                case OrderStatus.Completed:
                    message = OrderScriptMessage.CompletedMessage;
                    break;
                case OrderStatus.Cancelled:
                    message = OrderScriptMessage.CancelledMessage;
                    break;
                case OrderStatus.CustomerNotReceived:
                    message = OrderScriptMessage.CustomerNotReceivedMessage;
                    break;
                case OrderStatus.SellerDenied:
                    message = OrderScriptMessage.SellerDeniedMessage;
                    break;
                default:
                    message = "There're no option for this trigger";
                    break;
            }
            _dbContext.SaveChanges();

            return Ok(new
            {
                Message =  message,
                ProductId = _upchecked.Id
            });
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct([FromQuery]int id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound("Cannot found your product");

            product.HasDeleted = true;
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
            return Ok("Deleted");
        }

        // GET: api/product/categories
        [Authorize]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoryList([FromQuery]GetCategoryRequestDTO request)
        {
            try
            {
                var categories = _dbContext.Categories
                                .Where(c => c.Level == request.Level)
                                .AsQueryable();
                
                if (request.ParentId != null)
                {
                    categories = categories.Where(c => c.ParentCategoryId == request.ParentId);
                }
                
                // Filter layers
                if(request.Take.HasValue && request.Page.HasValue)
                {
                    categories = categories.Skip(((int)request.Page - 1) * (int)request.Take)
                    .Take((int)request.Take).AsQueryable();
                }
          
                var result = await categories.ProjectTo<GetCategoryResponseDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();


                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("categories/all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _dbContext.Categories
                    .ProjectTo<GetCategoryResponseDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (System.Exception exception)
            {
                return StatusCode(500 ,exception.Message); ;
            }
        }
        // GET: api/product/category
        [HttpGet("category")]
        public async Task<IActionResult> GetSingleCategory([FromQuery] int id)
        {
            try
            {
                var record = await _dbContext.Categories.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    Level = c.Level,
                    c.ParentCategoryId,
                    SubCategoryCount = c.SubCategories.Count
                }).FirstOrDefaultAsync(c => c.Id == id);

                return Ok(record);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/product/category
        [HttpPost("category")]
        public async Task<IActionResult> PostNewCategory([FromBody] AddCategoryRequestDTO request)
        {
            try
            {
                if(_dbContext.Categories.Any(p => p.Name == request.Name))
                {
                    return BadRequest("Your category has been existed");
                }

                var foundParentCategory = _dbContext.Categories.FirstOrDefault(p => p.Id == request.ParentCategoryId);

                request.ParentCategoryId = foundParentCategory?.Id;
                request.Level = foundParentCategory?.Level != null ? foundParentCategory.Level + 1 : 0;

                var category = _mapper.Map<AddCategoryRequestDTO, Category>(request);
               
                var newCategory = _dbContext.Categories.Add(category);

                _dbContext.SaveChanges();
                return Ok(_mapper.Map<GetCategoryResponseDTO>(category));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500 ,ex.Message);
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("category")]
        public async Task<IActionResult> UpdateSingleCategory([FromQuery]int id, [FromBody] UpdateCategoryRequestDTO request)
        {
            try {
                if (_dbContext.Categories.Any(p => p.Name == request.Name))
                {
                    return BadRequest("Your category has been existed");
                }

                var foundCategory = _dbContext.Categories
                                        .FirstOrDefault(p => p.Id == id);

                if(foundCategory.ParentCategoryId != request.ParentCategoryId)
                {
                    var foundParentCategory = _dbContext.Categories
                                                .FirstOrDefault(p => p.Id == request.ParentCategoryId);
                
                    request.Level = foundParentCategory?.Level != null ? foundParentCategory.Level + 1 : foundCategory.Level;
                }

                var updatedCategory = _mapper.Map(request, foundCategory);

                var newCategory = _dbContext.Categories.Update(updatedCategory);

                _dbContext.SaveChanges();

                return Ok(_mapper.Map<GetCategoryResponseDTO>(updatedCategory));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("category")]
        public async Task<IActionResult> DeleteSingleCategory([FromQuery]int id)
        {
            try
            {
                if(!_dbContext.Categories.Any(p => p.Id == id))
                {
                    return BadRequest("This item currently not exist");
                }
                await _unitOfWork.CategoryRepository.Delete(id);
                await _unitOfWork.CompleteAsync();
                return Ok("Deleted Item");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500 ,ex.Message);
            }
        }

        private IQueryable<Product> filterProductList(IQueryable<Product> productLstQuery,FilterProductRequestDTO filterProductRequestDTO)
        {
            if (filterProductRequestDTO.CategoryId != null)
            {
                productLstQuery = productLstQuery
                    .Where(product => product.ProductCategories.Any(pc => pc.CategoryId == filterProductRequestDTO.CategoryId))
                    .AsQueryable();
            }

            if (filterProductRequestDTO.Page != null || filterProductRequestDTO.Take != null)
            {
                productLstQuery = productLstQuery.Skip((filterProductRequestDTO.Page - 1) * filterProductRequestDTO.Take).Take(filterProductRequestDTO.Take).AsQueryable();
            }

            if(filterProductRequestDTO.MinPrice != null && filterProductRequestDTO.MaxPrice != null)
            {
                productLstQuery = productLstQuery.Where(p => p.Price >= filterProductRequestDTO.MinPrice && p.Price <= filterProductRequestDTO.MaxPrice).AsQueryable();
            }
            
            return productLstQuery;
        }
        private int countfilterProductList(IQueryable<Product> productLstQuery, FilterProductRequestDTO filterProductRequestDTO) {
            if (filterProductRequestDTO.CategoryId != null)
            {
                productLstQuery = productLstQuery
                    .Where(product => product.ProductCategories.Any(pc => pc.CategoryId == filterProductRequestDTO.CategoryId))
                    .AsQueryable();
            }

            if (filterProductRequestDTO.MinPrice != null && filterProductRequestDTO.MaxPrice != null)
            {
                productLstQuery = productLstQuery.Where(p => p.Price >= filterProductRequestDTO.MinPrice && p.Price <= filterProductRequestDTO.MaxPrice).AsQueryable();
            }

            return productLstQuery.Count();
        }


        // 3. Review Product
        [HttpGet("review/list")]
        public async Task<IActionResult> GetReviewProduct(int productId)
        {
            try
            {
                var foundReviewFromUser = _dbContext.Reviews.AsNoTracking()
                                                            .Include(p => p.User)
                                                            .Include(p => p.Replies)
                                                                .ThenInclude(p => p.UserPage)
                                                            .Where(p => p.ProductId == productId)
                                                            .AsQueryable();


                try
                {
                    var result = foundReviewFromUser.ProjectTo<ReviewProductResponseDTO>(_mapper.ConfigurationProvider).ToList();
                    //var result = _mapper.Map<ReviewProductResponseDTO>(foundReviewFromUser);
                    return Ok(new
                    {
                        Message = "Get success",
                        Result = result
                    });
                }
                catch (System.Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("review")]
        public async Task<IActionResult> CreateReviewProduct(ReviewProductCreationDTO request)
        {
            try
            {
                var foundReviewFromUser = _dbContext.Reviews.FirstOrDefault(p => p.UserId == request.UserId && p.ProductId == request.ProductId);

                var updatedReview = _mapper.Map(request, foundReviewFromUser);

                _dbContext.Reviews.Update(updatedReview);

                _dbContext.SaveChanges();

                return Ok(new
                {
                    Message = "Posted success",
                    Result = _mapper.Map<ReviewProductResponseDTO>(updatedReview)
                });
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("review/replies")]
        public async Task<IActionResult> GetReplyListFromReviewProduct(int reviewId)
        {
            try
            {
                var foundReviewFromUser = _dbContext.Replies.Where(p => p.ReviewId == reviewId);

                return Ok(new
                {
                    Message = "Get success",
                    Result = foundReviewFromUser.ProjectTo<ReplyReviewResponseDTO>(_mapper.ConfigurationProvider)
                });
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("review/reply")]  
        public async Task<IActionResult> CreateReplyReviewProduct(ReplyReviewCreationDTO request)
        {
            try
            {
                var foundReplyReviewFromUser = _dbContext.Reviews.FirstOrDefault(p => p.Id == request.ReviewId);

                if (foundReplyReviewFromUser is null)
                {
                    return BadRequest("Entity of this review doesn't exist");
                }

                var newReplyReview = _mapper.Map<Reply>(request);

                _dbContext.Replies.Add(newReplyReview);

                _dbContext.SaveChanges();

                return Ok(new
                {
                    Message = "Posted success",
                    Result = _mapper.Map<ReplyReviewResponseDTO>(newReplyReview)
                });
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }

}
