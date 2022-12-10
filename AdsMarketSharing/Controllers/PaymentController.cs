using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Payment;
using AdsMarketSharing.Entities;
using AdsMarketSharing.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper.QueryableExtensions;
using AdsMarketSharing.Services.Payment;

namespace AdsMarketSharing.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(SQLExpressContext dbContext, IPaymentService paymentService, IMapper mapper)
        {
            _dbContext = dbContext;
            this._paymentService = paymentService;
            _mapper = mapper;
        }

        [HttpGet("bankcode")]
        public async Task<IActionResult> GetBankList() {
            var bankList = System.Enum.GetValues(typeof(Enum.BankCode)).Cast<Enum.BankCode>().Select(p => p.ToString()).ToList();
            return Ok(bankList);
        }

        [HttpPost("invoice/create")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCreationDTO request, string? returnURL) {
            try
            {
                var createdInvoice = _mapper.Map<Invoice>(request);

                var vnPayRequest = await _paymentService.Checkout(createdInvoice, returnURL);
                //try
                //{
                //    //We will now define your HttpClient with your first using statement which will use a IDisposable.
                //    using (HttpClient client = new HttpClient())
                //    {
                //        //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
                //        //The HttpResponseMessage which contains status code, and data from response.
                //        using (HttpResponseMessage res = await client.GetAsync(vnPayRequest.Data))
                //        {
                //            //Then get the data or content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                //            using (HttpContent content = res.Content)
                //            {
                //                var data = await content.ReadAsStringAsync();
                //                if(data != null)
                //                {
                //                    var dataObj = JsonConvert.DeserializeObject<dynamic>(data);
                //                }
                //                else
                //                {
                //                    throw new Exception("There are no response from VNPay");
                //                }
                //            }
                //        }
                //    }
                //}
                //catch (Exception exception)
                //{
                //    return BadRequest(exception.Message);
                //}
                
                _dbContext.Invoices.Add(createdInvoice);
                _dbContext.SaveChanges();

                return Ok(new {
                    CheckoutUrl = vnPayRequest.Data,
                    Message= "Clicked to link",
                }) ;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmInvoice()
        {
            var VNPayResponse = await _paymentService.Confirm();

            if(VNPayResponse.StatusCode == 200 && VNPayResponse.Status == Enum.ResponseStatus.Successed)
            {
                var updatedInvoice = _dbContext.Invoices
                        .Include(p => p.Orders)
                            .ThenInclude(pc => pc.Product)
                        .Include(p => p.Orders)
                            .ThenInclude(pc => pc.Buyer)
                        .Include(p => p.Orders)
                            .ThenInclude(pc => pc.Merchant)
                        .Include(p => p.Orders)
                            .ThenInclude(pc => pc.Address)
                        .Include(p => p.Payment)
                        .FirstOrDefault(p => p.OnlineRef == VNPayResponse.Data.OnlineRef);

                if(updatedInvoice != null)
                {
                    updatedInvoice.HasPaid = true;

                    _dbContext.Invoices.Update(updatedInvoice);
                    _dbContext.SaveChanges();

                    return Ok(new
                    {
                        Message = "Completed payment",
                        Invoice = _mapper.Map<InvoiceResponseDTO>(updatedInvoice)
                    });
                }

                return BadRequest("Transaction is not included");

            }
            return StatusCode(VNPayResponse.StatusCode, VNPayResponse.Message);
        }

        [Authorize]
        [HttpGet("invoice/me")]
        public async Task<IActionResult> GetInvoice([FromQuery] int userId) {
            try
            {
                var dbInvoice = _dbContext.Invoices
                   .Include(p => p.Orders)
                       .ThenInclude(pc => pc.Product)
                       //.ThenInclude(pc => pc.Attachments)
                       //.ThenInclude(pc => pc.Attachment)
                   .Include(p => p.Orders)
                       .ThenInclude(pc => pc.Buyer)
                   .Include(p => p.Orders)
                       .ThenInclude(pc => pc.Merchant)
                   .Include(p => p.Orders)
                       .ThenInclude(pc => pc.Address)
                   .Include(p => p.Payment)
                   .Where(p => p.UserId == userId);

                return Ok(dbInvoice.ProjectTo<InvoiceResponseDTO>(_mapper.ConfigurationProvider).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
   
        }
        [Authorize]
        [HttpGet("order")]
        public async Task<IActionResult> GetOrderList([FromQuery] UserOrderFilterRequestDTO request)
        {
            var dbOrders = _dbContext.Orders.AsNoTracking().AsQueryable();

            if (int.TryParse(request.Pattern, out int searchNumber))
            {
                dbOrders = dbOrders.Where(p => p.InvoiceId != null ? p.InvoiceId == searchNumber : p.Id == searchNumber);
            }
            
            return Ok(dbOrders.ProjectTo<OrderResponseDTO>(_mapper.ConfigurationProvider).ToList());
        }
        [Authorize]
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId) {
            var foundOrder = _dbContext.Orders.AsNoTracking()
                                            .Include(p => p.Product)
                                            .ThenInclude(p => p.Attachments)
                                                .ThenInclude(p => p.Attachment)
                                            .Include(p => p.Invoice)
                                            .Include(p => p.Address)
                                            .Include(p => p.Buyer)
                                            .Include(p => p.Merchant)
                                            .FirstOrDefault(p => p.Id == orderId);

            if(foundOrder == null)
            {
                return BadRequest("Cannot find order");
            }
            var result = _mapper.Map<OrderResponseDTO>(foundOrder);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("order/seller")]
        public async Task<IActionResult> GetSellingOrder([FromQuery] OrderFilterRequestDTO filter)
        {
            var orders = _dbContext.UserPages
                            .Include(p => p.Orders)
                            .SelectMany(p => p.Orders)
                            .Where(p => p.MerchantId == filter.SellerId)
                            .OrderBy(p => (p.OrderStatus == OrderStatus.Pending) ? 0 :
                                            (p.OrderStatus == OrderStatus.Waiting) ? 1 :
                                            (p.OrderStatus == OrderStatus.Delivering) ? 2 :
                                            (p.OrderStatus == OrderStatus.Delivered) ? 3 :
                                            (p.OrderStatus == OrderStatus.Completed) ? 4 :
                                            (p.OrderStatus == OrderStatus.Cancelled) ? 5 :
                                            (p.OrderStatus == OrderStatus.CustomerNotReceived) ? 6 :
                                    7);
            return Ok(new
            {
                Message = "Get Selling Order List Successfully",
                Orders = orders
                .ProjectTo<OrderResponseDTO>(_mapper.ConfigurationProvider)
                .ToList()
            });
        }
    }
}
