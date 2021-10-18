﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class DepositMoneyCommand : MediatR.IRequest
    {
        public double Amount { get; set; }
        public string IbanCode { get; set; }
        public int? AccountId { get; set; }
        public string UniqueIdentifier { get; set; }
        public int? PersonId { get; set; }
        public string Currency { get; set; }

        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }
    
    }
}
