using AdsMarketSharing.Data;
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


        [HttpGet("")]
        public async Task<IActionResult> GetProductList([FromQuery]FilterProductRequestDTO filterProductRequestDTO)
        {
            var productLstQuery = _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Include(p => p.UserPages)
                .Include(p => p.ProductCategories)
                    .ThenInclude(p => p.Category)
                .AsQueryable();


            var result = await productLstQuery
                .ProjectTo<GetProductResponseDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if(result is null)
            {
                return NotFound("Not found the matches");
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductItem(int id)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Include(p => p.UserPages)
                .Include(p => p.ProductCategories)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(product is null)
            {
                return NotFound("Not Matches");
            }

            var result = _mapper.Map<GetProductResponseDTO>(product);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProductList(FilterProductRequestDTO filter)
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
            var productLst = _dbContext.Products.Where(p => p.UserPageId == userPage.Id).AsQueryable();

            var result = await productLst.ToListAsync();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewProduct([FromBody]AddProductRequestDTO request)
        {
            var product = _mapper.Map<Product>(request);

            try
            {
                if (product.ProductCategories is null)
                {
                    return NotFound("Cannot find your categories");
                }

                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, "created");
            }
            catch (System.Exception e)
            {
                return StatusCode(500,e.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody]AddProductRequestDTO request,[FromQuery] int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound("Not found matches");

            try
            {
                var newProduct = _mapper.Map(request, product);

               _dbContext.Update(newProduct);
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
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoryList([FromQuery]GetCategoryRequestDTO request)
        {
            try
            {
                var categories = _dbContext.Categories.Include(c=> c.SubCategories)
                .Where(c => c.Level == request.Level)
                .AsQueryable();

                if (request.ParentId != null)
                {
                    categories = categories.Where(c => c.ParentCategoryId == request.ParentId);
                }
                
                var result = await categories.Skip((request.Page - 1) * request.Take)
                    .Take(request.Take)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Level,
                        c.ParentCategoryId,
                        SubCategoryCount= c.SubCategories.Count
                    }).ToListAsync();

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
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
    }


}
