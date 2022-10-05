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
            var dbInvoice = _dbContext.Invoices
                            .Include(p => p.Orders)
                                .ThenInclude(pc => pc.Product)
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
    }
}
