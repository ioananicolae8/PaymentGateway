using Abstractions;
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
    public class PurchaseProductOperation : IWriteOperation<PurchaseProductCommand>
    {
        public IEventSender eventSender;
        public PurchaseProductOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(PurchaseProductCommand operation)
        {
            Database database = Database.GetInstance();
            Account account;
            Person person;
            if (operation.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.AccountId == operation.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == operation.IbanCode);
            }
            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.PersonId == operation.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == operation.UniqueIdentifier);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            var exists = database.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

            if (!exists)
            {
                throw new Exception("The person isn't associated with this account");

            }

            var totalAmount = 0.0;
            Product product;
            var pxts = new List<ProductXTransaction>();
            foreach (var item in operation.ProductDetails)
            {
                 product = database.Products.FirstOrDefault(x => x.ProductId == item.ProductId);

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

            database.ProductXTransaction.AddRange(pxts);

            ProductPurchased productPurchased = new ProductPurchased { ProductDetails = operation.ProductDetails };
            eventSender.SendEvent(productPurchased);

            database.SaveChanges();
        }
    }
}
