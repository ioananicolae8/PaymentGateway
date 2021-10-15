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
        private readonly IMediator _mediator;
        private readonly Database _database;
        public CreateProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }
        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
           
            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = (decimal)request.Limit;

            _database.Products.Add(product);

            ProductCreated eventProductCreated = new ProductCreated { Name = request.Name, Currency = request.Currency, Limit = request.Limit, Value = request.Value };

            _database.SaveChanges();
            await _mediator.Publish(eventProductCreated, cancellationToken);

            return Unit.Value;

        }
    }
}
