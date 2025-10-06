using CatalogSrevice.Api.Core.Domain;
using CatalogSrevice.Api.infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogSrevice.Api.infrastructure.EntityConfigurations
{
    public class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogBrand",CatalogContext.DEFAULT_SCHEMA);
            builder.HasKey(ci=>ci.Id);
            builder.Property(ci => ci.Id).UseHiLo("catalog_type_hilo").IsRequired();
            builder.Property(cb=>cb.Type).IsRequired().HasMaxLength(100);

            


        }
    }
}
