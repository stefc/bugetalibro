using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TXS.bugetalibro.Domain.Entities;

namespace TXS.bugetalibro.Infrastructure.Persistence.Configurations
{
    internal class AuszahlungEntityConfig : IEntityTypeConfiguration<Auszahlung>
    {
        public void Configure(EntityTypeBuilder<Auszahlung> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Betrag).HasConversion<decimal>().IsRequired();
            builder.Property(e => e.Datum).IsRequired();
            builder.Property(e => e.Notiz);

            builder.HasOne( e => e.Kategorie).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
