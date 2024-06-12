using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Domain.Approval_Requests.Enums;
using OutOfOffice.Domain.ApprovalRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Infrastructure.Data.EntityTypeConfiguration
{
    public class ApprovalRequestEntityConfiguration : IEntityTypeConfiguration<ApprovalRequest>
    {
        public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.Property(ar => ar.ApproverId)
               .IsRequired();

            builder.HasOne(ar => ar.Approver)
               .WithMany(x=>x.ApprovalRequests)
               .HasForeignKey(ar => ar.ApproverId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(ar => ar.LeaveRequestId)
               .IsRequired();

            builder.HasOne(ar => ar.LeaveRequest)
               .WithMany(x=>x.ApprovalRequests)
               .HasForeignKey(ar => ar.LeaveRequestId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(ar => ar.Status)
               .IsRequired()
               .HasDefaultValue(ApprovalRequestStatus.New)
               .HasConversion<string>()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore); 

            builder.Property(ar => ar.Comment)
               .HasMaxLength(1000); 
        }
    }
}