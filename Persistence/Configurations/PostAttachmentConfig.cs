using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    internal sealed class PostAttachmentConfig : IEntityTypeConfiguration<PostAttachment>
    {
        public void Configure(EntityTypeBuilder<PostAttachment> builder)
        {
            builder.ToTable("PostAttachments", AppDbContext.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(128);
            builder.Property(p => p.Url).IsRequired().HasMaxLength(256);
            builder.Property(p => p.ContentType).IsRequired().HasMaxLength(128);
        }
    }
}
