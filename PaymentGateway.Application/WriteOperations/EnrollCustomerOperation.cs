﻿using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Models;
using PaymentGateway.Data;
using PaymentGateway.PublishedLanguage.WritteSide;
using PaymentGateway.PublishedLanguage.Events;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        public IEventSender eventSender;
        public EnrollCustomerOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(EnrollCustomerCommand operation)
        {

            Database database = Database.GetInstance();
            Person person = new Person();
            person.Cnp = operation.UniqueIdentifier;
            person.Name = operation.Name;
            person.Type = operation.ClientType;
            var random = new Random();
            database.Persons.Add(person);

            if (operation.ClientType == "Company")
            {
                person.TypeOfPerson = PersonType.Company;
            }
            else if (operation.ClientType == "Individual")
            {
                person.TypeOfPerson = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            Account account = new Account();
            account.Type = operation.AccountType;

            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            database.Accounts.Add(account);

            database.SaveChanges();
            CustomerEnrolled eventCustomerEnroll = new(operation.Name, operation.UniqueIdentifier, operation.ClientType);
            eventSender.SendEvent(eventCustomerEnroll);

        }
    }
}