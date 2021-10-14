using Abstractions;
using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.Commands
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public PurchaseProductOperation(IMediator mediator, Database database)
        {
            _database = database;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            Database database = Database.GetInstance();
            Account account;
            Person person;
            if (request.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }
            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            var exists = _database.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

            if (!exists)
            {
                throw new Exception("The person isn't associated with this account");

            }

            var totalAmount = 0.0;
            Product product;
            var pxts = new List<ProductXTransaction>();
            foreach (var item in request.ProductDetails)
            {
                product = _database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Stock insufficient");
                }
                product.Limit -= item.Quantity;
                totalAmount += item.Quantity * product.Value;
                ProductXTransaction productXTransaction = new ProductXTransaction();
                productXTransaction.ProductId = item.ProductId;
                productXTransaction.Quantity = item.Quantity;
                productXTransaction.Value = product.Value;
                productXTransaction.Name = product.Name;
                pxts.Add(productXTransaction);
            }

            if (account.Balance < totalAmount)
            {
                throw new Exception("Insufficient money in your account");
            }

            Transaction transaction = new Transaction();
            transaction.Amount = -totalAmount;
            transaction.Currency = "$";
            database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in pxts)
            {
                item.TransactionId = transaction.TransactionId;
            }

            _database.ProductXTransaction.AddRange(pxts);

            ProductPurchased productPurchased = new ProductPurchased { ProductDetails = request.ProductDetails };

            _database.SaveChanges();

            await _mediator.Publish(productPurchased, cancellationToken);

            return Unit.Value;
        }
    }
}
