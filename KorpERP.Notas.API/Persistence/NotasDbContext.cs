using Microsoft.EntityFrameworkCore;
using KorpERP.Notas.API.Models;

namespace KorpERP.Notas.API.Persistence;

public class NotasDbContext : DbContext
{
    public NotasDbContext(DbContextOptions<NotasDbContext> options) : base(options)
    {
    }

    public DbSet<ProdutoProjection> Produtos { get; set; }
    public DbSet<Nota> Notas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Nota>()
            .HasMany(n => n.Itens)
            .WithOne(i => i.Nota)
            .HasForeignKey(i => i.NotaId);
    }
}