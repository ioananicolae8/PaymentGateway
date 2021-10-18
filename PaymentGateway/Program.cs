using Abstractions;
using PaymentGateway.Application.Commands;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using System;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;
using System.IO;
using PaymentGateway.Data;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation;
using MediatR.Pipeline;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.WebApi.MediatorPipeline;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
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
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            services.RegisterBusinessServices(Configuration);
            services.AddPaymentDataAccess(Configuration);

            services.Scan(scan => scan
                  .FromAssemblyOf<ListOfAccounts>()
                  .AddClasses(classes => classes.AssignableTo<IValidator>())
                  .AsImplementedInterfaces()
                  .WithScopedLifetime());


            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(CustomerEnrolled).Assembly);

            services.AddMediatR(new[] { typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var dataBase = serviceProvider.GetRequiredService<PaymentDbContext>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            var cnp = Guid.NewGuid().ToString().Substring(0,13);
            var iban = Guid.NewGuid().ToString();
            var iban2 = Guid.NewGuid().ToString();

            // use
            var enrollCustomer = new EnrollCustomerCommand
            {
                ClientType = "Company",
                AccountType = "Debit",
                Name = "Gigi Popa",
                Currency = "Eur",
                UniqueIdentifier = cnp,
                IbanCode = iban
            };

            ////var enrollCustomerOperation = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            ////enrollCustomerOperation.Handle(enrollCustomer, default).GetAwaiter().GetResult();
            await mediator.Send(enrollCustomer, cancellationToken);

            ///////////
            var accountCommand = new CreateAccountCommand
            {
                Balance = 45,
                Currency = "$",
                IbanCode = iban2,
                Type = "Economii",
                Status = "activ",
                Limit = 100000,
                UniqueIdentifier = cnp
            };
            // var accountOperation = serviceProvider.GetRequiredService<CreateAccountOperation>();
            // accountOperation.Handle(accountCommand, default).GetAwaiter().GetResult();
            await mediator.Send(accountCommand, cancellationToken);

            var deposityMoneyCommand = new DepositMoneyCommand
            {
                Amount = 3000000,
                Currency = "$",
                DateOfOperation = DateTime.Now,
                DateOfTransaction = DateTime.Now,
                IbanCode = iban2
            };
            // var deposityMoneyOperation = serviceProvider.GetRequiredService<DepositMoneyOperation>();
            // deposityMoneyOperation.Handle(deposityMoneyCommand, default).GetAwaiter().GetResult();
            await mediator.Send(deposityMoneyCommand, cancellationToken);

            var withdrawMoneyCommand = new WithdrawMoneyCommand
            {
                Amount = 200,
                Currency = "$",
                DateOfOperation = DateTime.Now,
                DateOfTransaction = DateTime.Now,
                IbanCode = iban2
            };
            //var withdrawMoneyOperation = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            //withdrawMoneyOperation.Handle(withdrawMoneyCommand, default).GetAwaiter().GetResult();
            await mediator.Send(withdrawMoneyCommand, cancellationToken);

            var productCommand = new CreateProductCommand
            {
                Name = "Banana",
                Value = 5,
                Currency = "RON",
                Limit = 400
            };
            //var createProductOperation = serviceProvider.GetRequiredService<CreateProductOperation>();
            //createProductOperation.Handle(productCommand, default).GetAwaiter().GetResult();
            await mediator.Send(productCommand, cancellationToken);

            var productCommand1 = new CreateProductCommand
            {
                Name = "Pere",
                Value = 4,
                Currency = "RON",
                Limit = 200
            };

            Product produs1 = new()
            {
                Name = "Pere",
                Value = 4,
                Currency = "RON",
                Limit = 200
            };
            dataBase.Products.Add(produs1);

            //var createProductOperation1 = serviceProvider.GetRequiredService<CreateProductOperation>();
            //createProductOperation1.Handle(productCommand1, default).GetAwaiter().GetResult();
            await mediator.Send(productCommand1, cancellationToken);
            var purchaseProductCommand = new PurchaseProductCommand()
            {
                IbanCode = iban2,
                UniqueIdentifier = cnp,
                ProductDetails = new System.Collections.Generic.List<PurchaseProductDetail>
            {
            new PurchaseProductDetail { ProductId = produs1.ProductId, Quantity = 2 }
            },
            };
            // var purchaseProductOperation = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            // purchaseProductOperation.Handle(purchaseProductCommand, default).GetAwaiter().GetResult();
            await mediator.Send(purchaseProductCommand, cancellationToken);

            var query = new ListOfAccounts.Query
            {
                Cnp = cnp
            };

            //var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            // var result = handler.Handle(query, default).GetAwaiter().GetResult();
            var result = await mediator.Send(query, cancellationToken);
        }
    }
}
