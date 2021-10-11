﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    public class Person
    {
        public int ? PersonId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Cnp { get; set; }
        public string Type { get; set; }

        public PersonType TypeOfPerson { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}