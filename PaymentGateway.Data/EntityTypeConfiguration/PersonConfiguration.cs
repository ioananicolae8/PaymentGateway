using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Data.EntityTypeConfiguration
{
    class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(u => new { u.PersonId });
            builder.Property(x => x.Name).HasMaxLength(255);
        }

        //public void Configure(EntityTypeBuilder<Person> builder)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
