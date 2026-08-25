using KorpERP.Produtos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KorpERP.Produtos.API.Persistence
{


    public class ProdutosDbContext : DbContext
    {
        public ProdutosDbContext(DbContextOptions<ProdutosDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<NotaProcessada> NotasProcessadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.Property(produto => produto.Versao)
                    .IsConcurrencyToken()
                    .HasDefaultValue(1L);
            });

            modelBuilder.Entity<NotaProcessada>(entity =>
            {
                entity.ToTable("notasProcessadas");
                entity.HasKey(nota => nota.NotaFiscalId)
                    .HasName("PK_notasProcessadas");
                entity.Property(nota => nota.NotaFiscalId)
                    .ValueGeneratedNever();
            });
        }
    }
}