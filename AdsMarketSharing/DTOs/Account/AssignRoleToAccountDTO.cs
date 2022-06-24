using AdsMarketSharing.Models.Auth;
using System.ComponentModel.DataAnnotations;

namespace AdsMarketSharing.DTOs.Account
{
    public class AssignRoleToAccountDTO
    {
        [Required(ErrorMessage = "Your role cannot be null")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "Your account cannot be null")]
        public int AccountId { get; set; }
    }
}
