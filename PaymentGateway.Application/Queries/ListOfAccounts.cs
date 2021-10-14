﻿using Abstractions;
using PaymentGateway.Data;
using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : IValidator<Query>
        {
            private readonly Database _database;

            public Validator(Database database)
            {
                _database = database;
            }

            public bool Validate(Query input)
            {
                var person = input.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.PersonId == input.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == input.Cnp);

                return person != null;
            }
        }
        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;
            private readonly IValidator<Query> _validator;

            public QueryHandler(Database database, IValidator<Query> validator)
            {
                _database = database;
                _validator = validator;
            }


            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var isValid = _validator.Validate(request);

                if (!isValid)
                {
                    throw new Exception("Person not found");
                }

                var person = request.PersonId.HasValue ?
                   _database.Persons.FirstOrDefault(x => x.PersonId == request.PersonId) :
                   _database.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

                var db = _database.Accounts.Where(x => x.PersonId == person.PersonId);
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.IbanCode,
                    AccountId = x.AccountId,
                    Limit = x.Limit,
                    Status = x.Status,
                    Type = x.Type
                }).ToList();
                return Task.FromResult(result);
            }
        }

        public class Model
        {
            public int? AccountId { get; set; }
            public double Balance { get; set; } = 0;
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public double Limit { get; set; }
            public int? PersonId { get; set; }
        }
    }
}