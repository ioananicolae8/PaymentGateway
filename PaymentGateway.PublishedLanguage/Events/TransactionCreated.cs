using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
   public class TransactionCreated : INotification
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public DateTime DateOfTransaction{get; set;}

        public TransactionCreated(double amount, string currency, DateTime date)
        {
            this.Amount = amount;
            this.Currency = currency;
            this.DateOfTransaction = date;
        }
    }
}
