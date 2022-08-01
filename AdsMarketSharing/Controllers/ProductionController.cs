using AdsMarketSharing.Data;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.DTOs.Product;
using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdsMarketSharing.Controllers
{
    [Authorize]
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(SQLExpressContext dbContext, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/<ValuesController>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoryList()
        {
            try
            {
                var products = await _unitOfWork.CategoryRepository.All();
                return Ok(products);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<ValuesController>/5
        [HttpGet("category")]
        public async Task<IActionResult> GetSingleCategory([FromQuery] int id)
        {
            try
            {
                var record = await _unitOfWork.CategoryRepository.GetById(id);
                return Ok(record);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<ValuesController>
        [HttpPost("category")]
        public async Task<IActionResult> PostNewCategory([FromBody] AddCategoryRequestDTO request)
        {
            try
            {
                var newProduct = await _unitOfWork.CategoryRepository.Upsert(_mapper.Map<AddCategoryRequestDTO, Category>(request, new Category()));

                await _unitOfWork.CompleteAsync();

                return Ok(newProduct);
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
                var updatedCategory = await _unitOfWork.CategoryRepository.Update(id, _mapper.Map(request,foundRecord));
                await _unitOfWork.CompleteAsync();
                return Ok(updatedCategory);
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
