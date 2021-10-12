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
    
    public class CreateAccountOperation : IWriteOperation<CreateAccountCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;
        private IEventSender eventSender1;

        public CreateAccountOperation(IEventSender eventSender1)
        {
            this.eventSender1 = eventSender1;
        }

        public CreateAccountOperation(IEventSender eventSender, AccountOptions accountOptions)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
        }

        public void PerformOperation(CreateAccountCommand operation)
        {
            Database database = Database.GetInstance();
            var account = new Account {
            Balance = _accountOptions.InitialBalance,
            Currency = operation.Currency,
            IbanCode = operation.IbanCode,
            Type = operation.Type,
            Status = operation.Status,
            Limit = operation.Limit
        };
            Person person;
            if (operation.PersonId.HasValue)
            {
                person = database.Persons?.FirstOrDefault(x => x.PersonId == operation.PersonId);
            }
            else
            {
                person = database.Persons?.FirstOrDefault(x => x.Cnp == operation.UniqueIdentifier);
                account.PersonId = person.PersonId;
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }
            account.PersonId = operation.PersonId;
            database.Accounts.Add(account);
            database.SaveChanges();

            AccountCreated eventAccountEvent = new(operation.IbanCode, operation.Type, operation.Status);
            _eventSender.SendEvent(eventAccountEvent);
        }
    }
}
