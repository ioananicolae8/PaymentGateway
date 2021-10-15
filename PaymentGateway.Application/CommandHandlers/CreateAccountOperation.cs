﻿using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.Commands
{

    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>
    {
        private readonly IMediator _mediator;
        private readonly Database _database;
        private readonly AccountOptions _accountOptions;

        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, Database database)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
        }
        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            
            var account = new Account
            {
                Balance = _accountOptions.InitialBalance,
                Currency = request.Currency,
                IbanCode = request.IbanCode,
                Type = request.Type,
                Status = request.Status,
                Limit = request.Limit
            };
            Person person;
            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId);
            }
            else
            {
                person = _database.Persons.FirstOrDefault(x => x.Cnp == request.UniqueIdentifier);
            }

            if (person == null)
            {
                throw new Exception("Person not found!");
            }
            account.PersonId = person.PersonId;
            _database.Accounts.Add(account);
            _database.SaveChanges();

            AccountCreated eventAccountEvent = new(request.IbanCode, request.Type, request.Status);

            await _mediator.Publish(eventAccountEvent, cancellationToken);
            return Unit.Value;
        }
    }
}