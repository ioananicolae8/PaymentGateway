using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class CreateAccountCommand : MediatR.IRequest
    {
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; }
        public string IbanCode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Limit { get; set; }
        public string UniqueIdentifier { get; set; }
        public int ? PersonId { get; set; }
    }
}
