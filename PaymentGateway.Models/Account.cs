using System;

namespace PaymentGateway.Models
{
    public class Account
    {
        public int? AccountId { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; }
        public string IbanCode { get;  set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Limit { get; set; }
        public int ? PersonId { get; set; }
    }
}
