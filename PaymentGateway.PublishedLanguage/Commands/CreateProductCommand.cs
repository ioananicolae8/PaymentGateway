using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class CreateProductCommand : MediatR.IRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }

    }
}
