using AdsMarketSharing.Models;
using AdsMarketSharing.Models.Email;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IMailService
    {
        Task<ServiceResponse<bool>> SendGmailAsync(MailContent emailContent);
        Task<ServiceResponse<bool>> SendSMTPAsync(MailContent smtpContent);
    }
}
