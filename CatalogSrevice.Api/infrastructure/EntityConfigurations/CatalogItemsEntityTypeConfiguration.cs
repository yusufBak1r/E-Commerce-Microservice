using CatalogSrevice.Api.Core.Domain;
using CatalogSrevice.Api.infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CatalogSrevice.Api.infrastructure.EntityConfigurations
{
    public class CatalogItemsEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Cakayog", CatalogContext.DEFAULT_SCHEMA);
            builder.Property(ci => ci.Id).UseHiLo("catalog_hilo").IsRequired();
            builder.Property(ci => ci.Name).IsRequired(true).HasMaxLength(50);
            builder.Property(ci => ci.Price).IsRequired(true);
            builder.Property(ci => ci.PictureFileName).IsRequired(true);
            builder.Ignore(ci => ci.PictureUri);
            builder.HasOne(ci=>ci.CatalogBrand).WithMany().HasForeignKey(ci => ci.CatalogBrandId);
            builder.HasOne(ci => ci.CatalogBrand).WithMany().HasForeignKey(ci => ci.CatalogTypeId);
        }
    }
}
