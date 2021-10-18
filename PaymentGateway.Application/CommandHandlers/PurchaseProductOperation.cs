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
        private readonly PaymentDbContext _dbContext;
        public PurchaseProductOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {

            Account account;
            if (request.AccountId.HasValue)
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }

            if (account == null)
            {
                throw new Exception("Account not found!");
            }

            double totalAmount = 0.0;
            Product product;
            var pxts = new List<ProductXTransaction>();

            Transaction transaction = new Transaction
            {
                Amount = -totalAmount,
                Currency = "$",
                DateTime = DateTime.Now,
                Status="OK",
                Type = "Purchase",
                AccountId=account.AccountId
            };
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            account.Balance -= totalAmount;

            foreach (var item in pxts)
            {
                item.TransactionId = transaction.TransactionId;
            }

            
            foreach (var item in request.ProductDetails)
            {
                product = _dbContext.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Stock insufficient");
                }
                product.Limit -= item.Quantity;
                totalAmount += (double)item.Quantity * product.Value;
                ProductXTransaction productXTransaction = new ProductXTransaction();
                productXTransaction.TransactionId = transaction.TransactionId;
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



            _dbContext.ProductXTransactions.AddRange(pxts);

            ProductPurchased productPurchased = new ProductPurchased { ProductDetails = request.ProductDetails };

            _dbContext.SaveChanges();

            await _mediator.Publish(productPurchased, cancellationToken);

            return Unit.Value;
        }
    }
}
