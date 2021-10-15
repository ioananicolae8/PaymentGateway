using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Models;
using PaymentGateway.Data;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.Commands
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        public EnrollCustomerOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {

            var random = new Random();

            var customer = new Person
            {
                Cnp = request.UniqueIdentifier,
                Name = request.Name
            };

            if (request.ClientType == "Company")
            {
                customer.TypeOfPerson = PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                customer.TypeOfPerson = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            customer.PersonId = _database.Persons.Count + 1;
            _database.Persons.Add(customer);

            Account account = new Account();
            account.Type = request.AccountType;
            account.Currency = request.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            _database.Accounts.Add(account);

            _database.SaveChanges();
            // CustomerEnrolled eventCustomerEnroll = new CustomerEnrolled(request.Name, request.UniqueIdentifier, request.ClientType);
            var eventCustomerEnroll = new CustomerEnrolled
            {
                Name = customer.Name,
                Cnp = customer.Cnp,
                ClientType = request.ClientType
            };

            await _mediator.Publish(eventCustomerEnroll, cancellationToken);
            return Unit.Value;
        }
    }
}
