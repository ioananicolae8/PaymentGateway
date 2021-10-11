using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public String Status { get; set; }
        public object Transactions { get; set; }
    }
}

