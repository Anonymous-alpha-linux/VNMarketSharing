using System;

namespace AdsMarketSharing.Models.Token
{
    public class TokenConfiguration<TData, TToken> 
        where TData : class
    {
        public TData TokenData { get; set; }
        public TToken TokenType { get; set; }
        public string TokenKey { get; set; }
        public DateTime ExpiresTime { get; set; } = DateTime.UtcNow.AddDays(3);
    }
}
