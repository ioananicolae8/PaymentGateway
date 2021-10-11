using Abstractions;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WritteSide;
using System;
using static PaymentGateway.PublishedLanguage.WritteSide.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static void Main(string[] args)
        {

            var db = Data.Database.GetInstance();

            Account firstAccount = new Account();
            firstAccount.Balance = 100;

            EnrollCustomerCommand client1 = new EnrollCustomerCommand();
            client1.Name = "Ana";
            client1.Currency = "Euro";
            client1.UniqueIdentifier = "2950603567835";
            client1.ClientType = "Individual";
            client1.AccountType = "Debit";

            IEventSender eventSender = new EventSender();

            EnrollCustomerOperation enrollOperation = new EnrollCustomerOperation(eventSender);
            enrollOperation.PerformOperation(client1);

            CreateAccountCommand accountCommand = new CreateAccountCommand();
            accountCommand.Balance = 45;
            accountCommand.Currency = "$";
            accountCommand.IbanCode = "ROING66434848993";
            accountCommand.Type = "Economii";
            accountCommand.Status = "activ";
            accountCommand.Limit = 100000;
            accountCommand.UniqueIdentifier = "2950603567835";
            IEventSender eventSender1 = new EventSender();
            CreateAccountOperation accountOperation = new CreateAccountOperation(eventSender1);
            accountOperation.PerformOperation(accountCommand);

            DepositMoneyCommand deposityMoneyCommand = new DepositMoneyCommand();
            deposityMoneyCommand.Amount = 3000000;
            deposityMoneyCommand.Currency = "$";
            deposityMoneyCommand.DateOfOperation = DateTime.Now;
            deposityMoneyCommand.DateOfTransaction = DateTime.Now;
            deposityMoneyCommand.UniqueIdentifier = "2950603567835";
            deposityMoneyCommand.IbanCode = "ROING66434848993";
            IEventSender eventSender2 = new EventSender();
            DepositMoneyOperation deposityMoneyOperation = new DepositMoneyOperation(eventSender2);
            deposityMoneyOperation.PerformOperation(deposityMoneyCommand);


            WithdrawMoneyCommand withdrawMoneyCommand = new WithdrawMoneyCommand();
            withdrawMoneyCommand.Amount = 200;
            withdrawMoneyCommand.Currency = "$";
            withdrawMoneyCommand.DateOfOperation = DateTime.Now;
            withdrawMoneyCommand.DateOfTransaction = DateTime.Now;
            withdrawMoneyCommand.UniqueIdentifier = "2950603567835";
            withdrawMoneyCommand.IbanCode = "ROING66434848993";
            IEventSender eventSender3 = new EventSender();
            WithdrawMoneyOperation withdrawMoneyOperation = new WithdrawMoneyOperation(eventSender3);
            withdrawMoneyOperation.PerformOperation(withdrawMoneyCommand);


            CreateProductCommand productCommand = new CreateProductCommand();
            productCommand.ProductId = 1;
            productCommand.Name = "Banana";
            productCommand.Value = 50;
            productCommand.Currency = "RON";
            productCommand.Limit = 5;
            CreateProductOperation createProductOperation = new CreateProductOperation(eventSender3);
            createProductOperation.PerformOperation(productCommand);

            CreateProductCommand productCommand1 = new CreateProductCommand();
            productCommand1.ProductId = 2;
            productCommand1.Name = "Pere";
            productCommand1.Value = 4;
            productCommand1.Currency = "RON";
            productCommand1.Limit = 9;
            CreateProductOperation createProductOperation1 = new CreateProductOperation(eventSender3);
            createProductOperation1.PerformOperation(productCommand1);

            PurchaseProductCommand purchaseProductCommand = new PurchaseProductCommand();
            purchaseProductCommand.IbanCode = "ROING66434848993";
            purchaseProductCommand.UniqueIdentifier = "2950603567835";
            purchaseProductCommand.ProductDetails = new System.Collections.Generic.List<PurchaseProductDetail>
            {
            new PurchaseProductDetail { ProductId =productCommand.ProductId , Quantity = 2 },
            new PurchaseProductDetail { ProductId = productCommand1.ProductId, Quantity = 6 }
            };

            PurchaseProductOperation purchaseProductOperation = new PurchaseProductOperation(eventSender);
            purchaseProductOperation.PerformOperation(purchaseProductCommand);
        }
    }
}
