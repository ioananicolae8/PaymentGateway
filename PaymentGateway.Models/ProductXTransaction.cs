﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class ProductXTransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }

        public double Quantity { get; set; }

        public double Value { get; set; }
        public string Name { get; set; }
    }
}