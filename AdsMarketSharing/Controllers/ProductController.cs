﻿using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.DTOs.Product;
using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Security.Claims;
using AdsMarketSharing.DTOs.File;
using Microsoft.AspNetCore.Http;
using AdsMarketSharing.DTOs;
using AdsMarketSharing.DTOs.Payment;

namespace AdsMarketSharing.Controllers
{
    //[Authorize]
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly SQLExpressContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(SQLExpressContext dbContext, IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        // 1. Product
        
        [HttpGet("")]
        public async Task<IActionResult> GetProductList([FromQuery]FilterProductRequestDTO filterProductRequestDTO)
        {
            var productLstQuery = _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Include(p => p.UserPage)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductClassifies)
                    .ThenInclude(pc => pc.ProductClassifyTypes)
                .Include(p => p.Attachments)
                .AsQueryable();

            int productMax = countfilterProductList(_dbContext.Products, filterProductRequestDTO);


            productLstQuery = filterProductList(productLstQuery,filterProductRequestDTO);

            var result = await productLstQuery
                .ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if(result is null)
            {
                return NotFound("Not found the matches");
            }

            return Ok(new GetProductListWithCount()
            {
                ProductList = result,
                Amount = productMax
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductItem(int id)
        {
            var product = _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Where(p => p.Id == id)
                .Include(p => p.UserPage)
                .Include(p => p.ProductCategories)
                    .ThenInclude(p => p.Category)
                .Include(p => p.ProductClassifies)
                    .ThenInclude(pc => pc.ProductClassifyTypes)
                .Include(p => p.Attachments);

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
                .Where(p => p.UserPageId == userPage.Id)
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

            return files.Select((file) => {
                if(file == null)
                {
                    return null;
                }
                return _fileStorageService.CreateFolderAndSaveImage(file.FileName, file.OpenReadStream(), usernameStr).Result.Data;
            }).ToList();
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewProduct([FromForm]AddProductRequestDTO request)
        {
            // 1. Handle IFormFile
            request.Attachments = await CreateFolderAndSaveImage(request.Files);


            // 2. Proper the product details
            var detailImageLst = request.ProductDetails.Select(d => d.Image).Where(image => image != null).ToList();
            var attachments = await CreateFolderAndSaveImage(detailImageLst);
            
            request.ProductDetails = request.ProductDetails.Select((d, index) =>
            {
                d.PresentImage = attachments.ElementAtOrDefault(index);
                return d;

            }).ToList();

            // 2. Upload product
            var product = _mapper.Map<Product>(request);
            var classifyTypes = product.ProductClassifies.Select(pc => pc.ProductClassifyTypes).ToArray();

            // 3. Assign classify detail
            List<ProductClassfiyDetail> classifyDetail = null;
            if (classifyTypes != null)
            {
                 classifyDetail = request.ProductDetails
                    .AsQueryable()
                    .Select(pd => new ProductClassfiyDetail()
                    {
                        Price = pd.Price,
                        Inventory = pd.Inventory,
                        PresentImage = _mapper.Map<Attachment>(pd.PresentImage),
                        ClassifyTypeKey = classifyTypes.ElementAtOrDefault(0).ElementAtOrDefault(pd.ClassifyIndexes.ElementAt(0)),
                        ClassifyTypeValue = classifyTypes.ElementAtOrDefault(1) != null ? classifyTypes.ElementAtOrDefault(1).ElementAtOrDefault(pd.ClassifyIndexes.ElementAt(1)): null,
                    })
                    .ToList();
            }
            
            try
            {
                if (product.ProductCategories is null)
                {
                    return NotFound("Cannot find your categories");
                }

                _dbContext.Add(product);
                if(classifyDetail != null)
                {
                    _dbContext.AddRange(classifyDetail);
                }
                _dbContext.SaveChanges();
                return StatusCode(201, "created");
            }
            catch (System.Exception e)
            {
                return StatusCode(500,e.Message);
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
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct([FromQuery]int id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound("Cannot found your product");

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
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
                if(request.ParentCategoryId != null)
                {
                    var foundCategory = await _unitOfWork.CategoryRepository.GetById(request.ParentCategoryId ?? -1);
                    if(foundCategory != null)
                    {
                        request.Level = foundCategory.Level + 1;
                    }
                    else
                    {
                        request.ParentCategoryId = null;
                    }
                }

                var category = _mapper.Map<AddCategoryRequestDTO, Category>(request);

                var newCategory = await _unitOfWork.CategoryRepository.Add(category);

                await _unitOfWork.CompleteAsync();


                if (newCategory is null)
                {
                    return StatusCode(500,"Cannot add now in server side");
                }

                return Ok(newCategory);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("category")]
        public async Task<IActionResult> UpdateSingleCategory([FromQuery]int id, [FromBody] UpdateCategoryRequestDTO request)
        {
            try
            {
                var foundRecord = await _unitOfWork.CategoryRepository.GetById(id);
                var newCategory = await _unitOfWork.CategoryRepository.Update(id, _mapper.Map(request,foundRecord));
                await _unitOfWork.CompleteAsync();

                return Ok(newCategory);
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
                await _unitOfWork.CategoryRepository.Delete(id);
                await _unitOfWork.CompleteAsync();
                return Ok("Deleted Item");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
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
    }


}
