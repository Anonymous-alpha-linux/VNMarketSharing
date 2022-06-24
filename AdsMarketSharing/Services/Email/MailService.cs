using AdsMarketSharing.Interfaces;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using AdsMarketSharing.Models.Email;
using Microsoft.Extensions.Options;
using AdsMarketSharing.Models;

namespace AdsMarketSharing.Services.Email
{
    public class MailService : IMailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public MailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }
        public async Task<ServiceResponse<bool>> SendGmailAsync(MailContent emailContent)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                MailMessage message = new MailMessage();
                // 2. 
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.To.Add(new MailAddress(emailContent.To));
                message.IsBodyHtml = emailContent.IsHtmlBody;
                message.Sender = new MailAddress(_emailConfiguration.EmailAddress, emailContent.DisplayName);
                message.From = new MailAddress(_emailConfiguration.EmailAddress, emailContent.DisplayName);
                message.Body = emailContent.HtmlBody;
                message.Subject = emailContent.Subject;

                //message.ReplyToList.Add(new MailAddress(emailContent.From));

                using var smtpClient = new SmtpClient()
                {
                    Port= _emailConfiguration.Port,
                    UseDefaultCredentials = true,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_emailConfiguration.EmailAddress,_emailConfiguration.Password),
                    Host = _emailConfiguration.Host
                };
                await smtpClient.SendMailAsync(message);

                // [*] Response definition
                response.Data = true;
                response.Message = "Send Email Successfully";
                response.ServerMessage = $"Sent email to {emailContent.To}";
                response.Status = Enum.ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (System.Exception e)
            {
                response.Data = false;
                response.Message = "Failed to send";  
                response.ServerMessage = e.Message;
                response.Status = Enum.ResponseStatus.Failed;
                response.StatusCode = 403;
            }
            return response;
        }
        public async Task<ServiceResponse<bool>> SendSMTPAsync(MailContent smtpContent) {
            var response = new ServiceResponse<bool>();
            try
            {
                // 1. 
                SmtpClient smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(_emailConfiguration.UserName, _emailConfiguration.Password),
                    EnableSsl = true
                };
                // 2.
                smtpClient.Send(smtpContent.From, smtpContent.To, smtpContent.Subject, smtpContent.Body);
                // [*] Response definition
                response.Data = true;
                response.Message = "Send Email Successfully";
                response.ServerMessage = $"Sent email to {smtpContent.To}";
                response.Status = Enum.ResponseStatus.Successed;
                response.StatusCode = 200;
            }
            catch (System.Exception e)
            {
                response.Data = false;
                response.Message = "Failed to send";
                response.ServerMessage = e.Message;
                response.Status = Enum.ResponseStatus.Failed;
                response.StatusCode = 403;
            }
            return response;
        }
    }
}
