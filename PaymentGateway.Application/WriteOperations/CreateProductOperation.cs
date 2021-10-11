﻿using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WritteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateProductOperation : IWriteOperation<CreateProductCommand>
    {
        public IEventSender eventSender;
        public CreateProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(CreateProductCommand operation)
        {
            Database database = Database.GetInstance();
            Product product = new Product();
            product.ProductId = operation.ProductId;
            product.Name = operation.Name;
            product.Value = operation.Value;
            product.Currency = operation.Currency;
            product.Limit = operation.Limit;

            database.Products.Add(product);

            ProductCreated eventProductCreated = new ProductCreated { Name = operation.Name, Currency = operation.Currency, Limit = operation.Limit, Value = operation.Value };
            eventSender.SendEvent(eventProductCreated);

            database.SaveChanges();

        }
    }
}
