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
        private readonly PaymentDbContext _dbContext;
        public EnrollCustomerOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
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
                customer.TypeOfPerson = (int)PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                customer.TypeOfPerson = (int)PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            _dbContext.Persons.Add(customer);
            _dbContext.SaveChanges();

            var account = new Account()
            {
                Type = request.AccountType,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = request.IbanCode,
                Status = "Active",
                PersonId = customer.PersonId
            };

            _dbContext.Accounts.Add(account);

            _dbContext.SaveChanges();
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
