using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
   public  class CustomerEnrolled : INotification
    {
        public string Name { get; set; } 
        public string Cnp { get; set; }
        public string ClientType { get; set; }

        //public CustomerEnrolled(string name, string cnp, string clientType)
        //{
        //    this.Name = name;
        //    this.Cnp = cnp;
        //    this.ClientType = clientType;
        //}

        public override string ToString()
        {
            return Name + " "+ Cnp+ " " + ClientType;
        }
    }
}
