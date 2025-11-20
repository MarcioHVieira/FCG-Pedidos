using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Api.Domain.Entities;

namespace Pedidos.Api.Infrastructure.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UsuarioNome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(p => p.JogoTitulo)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(p => p.Valor)
                .IsRequired()
                .HasColumnType("numeric(8,2)");

            builder.ToTable("Pedidos");
        }
    }
}