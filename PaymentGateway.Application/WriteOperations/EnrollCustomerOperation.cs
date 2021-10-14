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
        public IEventSender eventSender;
        public EnrollCustomerOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

            public Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {

            Database database = Database.GetInstance();
            //Person person = new Person();
            //person.Cnp = operation.UniqueIdentifier;
            //person.Name = operation.Name;
            //person.Type = operation.ClientType;
            var random = new Random();

            var customer = new Person
            {
                Cnp = request.UniqueIdentifier,
                Name = request.Name,
                Type = request.AccountType
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

            customer.PersonId = database.Persons.Count + 1;
            database.Persons.Add(customer);

            Account account = new Account();
            account.Type = request.AccountType;
            account.Currency = request.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            database.Accounts.Add(account);

            database.SaveChanges();
            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            eventSender.SendEvent(eventCustomerEnroll);
            return Unit.Task;
        }
    }
}
