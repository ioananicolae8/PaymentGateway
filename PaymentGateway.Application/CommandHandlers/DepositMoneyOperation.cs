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

    public class DepositMoneyOperation : IRequestHandler<DepositMoneyCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _dbContext;
        public DepositMoneyOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {

            Account account;

            if (request.AccountId.HasValue)
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.AccountId == request.AccountId);
            }
            else
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.IbanCode == request.IbanCode);
            }
            
            if (account == null)
            {
                throw new Exception("Account not found!");
            }
            
            
            Transaction transaction = new Transaction
            {
                AccountId = account.AccountId,
                Currency = request.Currency,
                DateTime = request.DateOfTransaction,
                Amount = request.Amount,
                Status = "OK",
                Type = "Deposit"
            };
            account.Balance += request.Amount;


            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            TransactionCreated eventTransactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            AccountUpdated eventAccountUpdated = new AccountUpdated(request.IbanCode, request.DateOfOperation, request.Amount);

            await _mediator.Publish(eventTransactionCreated, cancellationToken);
            await _mediator.Publish(eventAccountUpdated, cancellationToken);

            return Unit.Value;
        }
    }
}
