using System;

namespace AdsMarketSharing.Models
{
    public class ServiceResponseException<TResponse> : Exception
    {
        public ServiceResponseException(int statusCode)
        {
            StatusCode = statusCode;
        }
        public ServiceResponseException(int statusCode,TResponse value)
        {
            StatusCode = statusCode;
            Value = value;
        }
        public ServiceResponseException(int statusCode, TResponse value, string message) : base(message)
        {
            StatusCode = statusCode;
            Value = value;
        }
        public ServiceResponseException(int statusCode, TResponse value,object additionalValue, string message) : base(message)
        {
            StatusCode = statusCode;
            Value = value;
            AdditionalValue = additionalValue;
        }
        public int StatusCode { get;}
        public TResponse Value { get; }
        public object AdditionalValue { get; }
    }
}
