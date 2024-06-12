using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutOfOffice.Domain.Employees;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Infrastructure.Data.EntityTypeConfiguration
{
    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.Property(e => e.FullName)
             .IsRequired()
             .HasMaxLength(100);

            builder.Property(e => e.OutOfOfficeBalance)
              .IsRequired();

            builder.Property(e => e.Status)
             .HasConversion<string>()
             .IsRequired();

            builder.Property(e => e.Position)
             .HasConversion<string>()
             .IsRequired();

            builder.Property(e => e.Subdivision)
             .HasConversion<string>()
             .IsRequired();

            builder.HasOne(e => e.PeoplePartner)
               .WithMany()
               .HasForeignKey(e => e.PeoplePartnerID)
               .OnDelete(DeleteBehavior.NoAction);
            builder.Property(e => e.Photo)
               .IsRequired(false);
        }
    }
}