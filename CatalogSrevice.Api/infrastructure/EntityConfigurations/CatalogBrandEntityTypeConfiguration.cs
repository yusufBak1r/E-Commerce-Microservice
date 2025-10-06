using CatalogSrevice.Api.Core.Domain;
using CatalogSrevice.Api.infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogSrevice.Api.infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand", CatalogContext.DEFAULT_SCHEMA);
            builder.HasKey(ci => ci.Id);
            builder.Property(ci=>ci.Id).UseHiLo("catalog_breand_hilo").IsRequired();
            builder.Property(cb => cb.Brand).IsRequired().HasMaxLength(100);
        }
    }
}
