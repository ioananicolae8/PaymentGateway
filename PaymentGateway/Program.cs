using Abstractions;
using PaymentGateway.Application.Commands;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.ReadOperations;
using System;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;
using System.IO;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Data;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static void Main(string[] args)
        {

            //var db = Database.GetInstance();

            //Account firstAccount = new Account();
            //firstAccount.Balance = 100;

            //EnrollCustomerCommand client1 = new EnrollCustomerCommand();
            //client1.Name = "Ana";
            //client1.Currency = "Euro";
            //client1.UniqueIdentifier = "2950603567835";
            //client1.ClientType = "Individual";
            //client1.AccountType = "Debit";

            //IEventSender eventSender = new EventSender();

            //EnrollCustomerOperation enrollOperation = new EnrollCustomerOperation(eventSender);
            //enrollOperation.PerformOperation(client1);

            Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            // setup
            var services = new ServiceCollection();
            services.RegisterBusinessServices(Configuration);

            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Company",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "Eur",
                UniqueIdentifier = "23"
            };

            var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            enrollCustomerOperation.Handle(enrollCustomer, default).GetAwaiter().GetResult();

            ///////////
            var accountCommand = new CreateAccountCommand {
                Balance = 45,
                Currency = "$",
                IbanCode = "ROING66434848993",
                Type = "Economii",
                Status = "activ",
                Limit = 100000,
                UniqueIdentifier = "2950603567835"
            };
            var accountOperation = serviceProvider.GetRequiredService<CreateAccountOperation>();
            accountOperation.Handle(accountCommand, default).GetAwaiter().GetResult();

            var deposityMoneyCommand = new DepositMoneyCommand {
                Amount = 3000000,
                Currency = "$",
                DateOfOperation = DateTime.Now,
                DateOfTransaction = DateTime.Now,
                UniqueIdentifier = "2950603567835",
                IbanCode = "ROING66434848993"
            };
            var deposityMoneyOperation = serviceProvider.GetRequiredService<DepositMoneyOperation>();
            deposityMoneyOperation.Handle(deposityMoneyCommand, default).GetAwaiter().GetResult();


            var withdrawMoneyCommand = new WithdrawMoneyCommand {
                Amount = 200,
                Currency = "$",
                DateOfOperation = DateTime.Now,
                DateOfTransaction = DateTime.Now,
                UniqueIdentifier = "2950603567835",
                IbanCode = "ROING66434848993"
            };
            var withdrawMoneyOperation = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            withdrawMoneyOperation.Handle(withdrawMoneyCommand, default).GetAwaiter().GetResult();


            var productCommand = new CreateProductCommand {
                ProductId = 1,
                Name = "Banana",
                Value = 50,
                Currency = "RON",
                Limit = 5
            };
            var createProductOperation = serviceProvider.GetRequiredService<CreateProductOperation>();
            createProductOperation.Handle(productCommand, default).GetAwaiter().GetResult();

            var productCommand1 = new CreateProductCommand {
                ProductId = 2,
                Name = "Pere",
                Value = 4,
                Currency = "RON",
                Limit = 9
            };
            var createProductOperation1 = serviceProvider.GetRequiredService<CreateProductOperation>();
            createProductOperation1.Handle(productCommand1, default).GetAwaiter().GetResult();

            var purchaseProductCommand = new PurchaseProductCommand() {
            IbanCode = "ROING66434848993",
            UniqueIdentifier = "2950603567835",
            ProductDetails = new System.Collections.Generic.List<PurchaseProductDetail>
            {
            new PurchaseProductDetail { ProductId =productCommand.ProductId , Quantity = 2 },
            new PurchaseProductDetail { ProductId = productCommand1.ProductId, Quantity = 6 }
            },
        };
            var purchaseProductOperation = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            purchaseProductOperation.Handle(purchaseProductCommand, default).GetAwaiter().GetResult();


            var query = new Application.ReadOperations.ListOfAccounts.Query
            {
                PersonId = 1
            };

            var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            var result = handler.Handle(query, default).GetAwaiter().GetResult();
        }
    }
}
