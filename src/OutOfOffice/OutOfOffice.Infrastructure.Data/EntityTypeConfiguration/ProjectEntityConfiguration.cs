using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using OutOfOffice.Domain.Projects.Enums;
using OutOfOffice.Domain.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutOfOffice.Infrastructure.Data.EntityTypeConfiguration
{
    public class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(c => c.Id)
               .IsRequired()
               .ValueGeneratedOnAdd()
               .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.Property(p => p.ProjectType)
               .IsRequired()
               .HasConversion<string>();

            builder.Property(p => p.StartDate)
               .IsRequired();

            builder.Property(p => p.EndDate)
               .IsRequired(false);

            builder.Property(p => p.ProjectManagerId)
               .IsRequired();

            builder.HasOne(p => p.ProjectManager)
               .WithMany()
               .HasForeignKey(p => p.ProjectManagerId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(p => p.Comment)
               .HasMaxLength(1000);  

            builder.Property(p => p.Status)
               .IsRequired()
               .HasDefaultValue(ProjectStatus.Active)
               .HasConversion<string>()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
