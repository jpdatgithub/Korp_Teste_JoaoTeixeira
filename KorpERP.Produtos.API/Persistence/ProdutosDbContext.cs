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
    }
}