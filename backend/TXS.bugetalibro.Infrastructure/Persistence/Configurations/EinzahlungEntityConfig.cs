using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Infrastructure.Persistence.Configurations
{
    internal class EinzahlungEntityConfig : IEntityTypeConfiguration<Einzahlung>
    {
        public void Configure(EntityTypeBuilder<Einzahlung> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Betrag).IsRequired();
            builder.Property(e => e.Datum).IsRequired();
        }
    }
}
