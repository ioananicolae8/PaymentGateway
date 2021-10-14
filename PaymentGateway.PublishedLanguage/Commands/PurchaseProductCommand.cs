﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class PurchaseProductCommand: MediatR.IRequest
    {
        public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();

        public int? PersonId { get; set; }
        public string UniqueIdentifier { get; set; }

        public int? AccountId { get; set; }
        public string   IbanCode { get; set; }
        public class PurchaseProductDetail
        {
            public int ProductId { get; set; }
            public double Quantity { get; set; }
        }


    }
}
