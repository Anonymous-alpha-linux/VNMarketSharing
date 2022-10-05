using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsMarketSharing.Controllers
{
    [Route("checkout")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class VNPaymentController : Controller
    {
        // GET: VNPaymentController
        public ActionResult Index(object request)
        {
            return View();
        }
    }
}
