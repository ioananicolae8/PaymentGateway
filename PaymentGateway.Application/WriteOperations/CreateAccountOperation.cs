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
        public IEventSender eventSender;

        public CreateAccountOperation()
        {
        }

        public CreateAccountOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(CreateAccountCommand operation)
        {
            Database database = Database.GetInstance();
            Account account = new Account();
            account.Balance = operation.Balance;
            account.Currency = operation.Currency;
            account.IbanCode = operation.IbanCode;
            account.Type = operation.Type;
            account.Status = operation.Status;
            account.Limit = operation.Limit;
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
            eventSender.SendEvent(eventAccountEvent);
        }
    }
}
