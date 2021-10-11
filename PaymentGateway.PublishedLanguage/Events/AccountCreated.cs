using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated
    {
        public string IbanCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public AccountCreated (string IbanCode, string Type, string Status)
        {
            this.IbanCode = IbanCode;
            this.Type = Type;
            this.Status = Status;
        }
    }
}
