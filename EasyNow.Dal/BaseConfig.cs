using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyNow.Dal
{
    public abstract class BaseConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        { 
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(t => t.CreateTime).IsRequired().ValueGeneratedOnAdd();
            builder.Property(t => t.UpdateTime).IsRequired().ValueGeneratedOnAddOrUpdate();
        }
    }
}