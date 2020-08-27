using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Infrastructure.Persistence.Configurations
{
    internal class KategorieEntityConfig : IEntityTypeConfiguration<Kategorie>
    {
        public void Configure(EntityTypeBuilder<Kategorie> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name).IsRequired();
        }
    }
}
