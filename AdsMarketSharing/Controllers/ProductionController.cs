using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.DTOs.Product;
using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdsMarketSharing.Controllers
{
    [Authorize]
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




        [HttpGet("")]
        public async Task<IActionResult> GetProductList()
        {
            return Ok();
        }
        [HttpGet("")]
        public async Task<IActionResult> GetProductItem()
        {
            return Ok();
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewProduct()
        {
            return Ok();
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdateProduct()
        {
            return Ok();
        }
        [HttpDelete("")]
        public async Task<IActionResult> DeleteProduct()
        {
            return Ok();
        }
    }


}
