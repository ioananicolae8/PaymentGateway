using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountUpdated : INotification
    {
        public string IbanCode { get; set; }
        public DateTime DateOfOperation { get; set; }
        public double Amount { get; set; }

        public AccountUpdated(string ibanCode, DateTime date, double amount)
        {
            this.IbanCode = ibanCode;
            this.DateOfOperation = date;
            this.Amount = amount;
        }
    }
   
}
