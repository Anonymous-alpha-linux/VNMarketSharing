namespace AdsMarketSharing.Models.Email
{
    public class MailContent
    {
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public string Subject { get; set; }
        public bool IsHtmlBody { get; set; } = true;
    }
}
