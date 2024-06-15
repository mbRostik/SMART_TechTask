using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Domain.Leave_Requests.Enums;
using OutOfOffice.Domain.Leave_Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OutOfOffice.Infrastructure.Data.EntityTypeConfiguration
{
    public class LeaveRequestEntityConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.Property(lr => lr.EmployeeId)
               .IsRequired();

            builder.HasOne(lr => lr.Employee)
               .WithMany(x => x.LeaveRequests)
               .HasForeignKey(lr => lr.EmployeeId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(lr => lr.AbsenceReason)
               .IsRequired()
               .HasConversion<string>();

            builder.Property(lr => lr.StartDate)
               .IsRequired();

            builder.Property(lr => lr.EndDate)
               .IsRequired();

            builder.Property(lr => lr.Comment)
               .HasMaxLength(1000);

            builder.Property(lr => lr.Status)
               .IsRequired()
               .HasDefaultValue(LeaveRequestStatus.Submited)
               .HasConversion<string>();
        }
    }
}