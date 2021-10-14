using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.Commands
{

    public class CreateProductOperation : IRequestHandler<CreateProductCommand>
    {
        public IEventSender eventSender;
        public CreateProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Database database = Database.GetInstance();
            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = request.Limit;

            database.Products.Add(product);

            ProductCreated eventProductCreated = new ProductCreated { Name = request.Name, Currency = request.Currency, Limit = request.Limit, Value = request.Value };
            eventSender.SendEvent(eventProductCreated);

            database.SaveChanges();
            return Unit.Task;

        }
    }
}
