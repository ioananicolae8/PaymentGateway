using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace PaymentGateway.Application.Commands
{
    public class WithdrawMoneyOperation : IRequestHandler<WithdrawMoneyCommand>
    {
        public IEventSender eventSender;
        private readonly Database _database;
        public WithdrawMoneyOperation(IEventSender eventSender, Database database)
        {
            _database = database;
        }

        public Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
        {
            // Database database = Database.GetInstance();
            Account account;
            Person person;
            if (request.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }
            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }
            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            if (person == null)
            {
                throw new Exception("Person not found!");
            }

            var exists = _database.Accounts.Any(x => x.PersonId == person.PersonId && x.AccountId == account.AccountId);

            if (!exists)
            {
                throw new Exception("The person isn't associated with this account");

            }
            account.AccountId = request.AccountId;
            person.PersonId = request.PersonId;
            Transaction transaction = new Transaction();
            transaction.Currency = request.Currency;
            transaction.Date = request.DateOfTransaction;
            transaction.Amount = -request.Amount;
            account.Balance -= request.Amount;

            _database.Transactions.Add(transaction);

            TransactionCreated eventTransactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            eventSender.SendEvent(eventTransactionCreated);
            AccountUpdated eventAccountUpdated = new AccountUpdated(request.IbanCode, request.DateOfOperation, request.Amount);
            eventSender.SendEvent(eventAccountUpdated);

            _database.SaveChanges();
            return Unit.Task;

        }
    }
}
