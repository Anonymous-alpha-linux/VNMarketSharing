using System;
using Microsoft.AspNetCore.Mvc;
using AdsMarketSharing.Services.Ads;
using Microsoft.AspNetCore.Authorization;
using AdsMarketSharing.Interfaces;
using AdsMarketSharing.Models.ServiceUser;

namespace AdsMarketSharing.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CollaboratorController : Controller
    {
        private readonly IAds _AdsService;

        public CollaboratorController(IAds adsService)
        {
            this._AdsService = adsService;
        }
        private Collaborator firstPerson = new Collaborator()
        {
            Id = 1,
            FirstName = "Organization",
            LastName = "A",
            CreatedAt = DateTime.Now,
            Hosting_URL = "abc.com",
            Port = 8000,
            CurrentStatus = Enum.CollaboratorStatus.Annual
        };

        [HttpGet]
        public ActionResult<Collaborator> GetCollaborator()
        {
            return StatusCode(201,firstPerson);
        }
    }
}
