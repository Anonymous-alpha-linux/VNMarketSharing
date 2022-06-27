using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Services.Ads;
using AdsMarketSharing.Models.ServiceUser;
using AdsMarketSharing.Data;

namespace AdsMarketSharing.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/collaborator")]
    public class CollaboratorController : Controller
    {
        private readonly IAds _adsService;
        private readonly SQLExpressContext _context;

        public CollaboratorController(IAds adsService,SQLExpressContext context)
        {
            this._adsService = adsService;
            this._context = context;
        }

        [HttpGet]
        public ActionResult<Collaborator> GetCollaborator()
        {
            return StatusCode(200);
        }

        [HttpPost("create")]
        public IActionResult AddCollaborator()
        {
            return Ok();
        }

        [HttpPost("update")]
        public IActionResult UpdateCollaborator()
        {
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult DeleteCollaborator()
        {
            return Ok();
        }
    }
}
