using Abstractions;
using System;

namespace PaymentGateway.ExternalService
{
    public class EventSender:IEventSender
    {
       public void SendEvent(object e)
        {
            //Console.WriteLine(e.ToString());
            Console.WriteLine("Event" + e.GetType().FullName);
        }
    }
}
