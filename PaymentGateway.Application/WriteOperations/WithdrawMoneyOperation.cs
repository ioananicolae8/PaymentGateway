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
    public class WithdrawMoneyOperation : IWriteOperation<WithdrawMoneyCommand>
    {
        public IEventSender eventSender;
        public WithdrawMoneyOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(WithdrawMoneyCommand operation)
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
            account.AccountId = operation.AccountId;
            person.PersonId = operation.PersonId;
            Transaction transaction = new Transaction();
            transaction.Currency = operation.Currency;
            transaction.Date = operation.DateOfTransaction;
            transaction.Amount = -operation.Amount;
            account.Balance -= operation.Amount;

            database.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(operation.Amount, operation.Currency, operation.DateOfTransaction);
            eventSender.SendEvent(eventTransactionCreated);
            AccountUpdated eventAccountUpdated = new AccountUpdated(operation.IbanCode, operation.DateOfOperation, operation.Amount);
            eventSender.SendEvent(eventAccountUpdated);

            database.SaveChanges();

        }
    }
}
