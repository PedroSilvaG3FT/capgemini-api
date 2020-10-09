
using Microsoft.EntityFrameworkCore;
namespace capgemini_api.Models
{
    public class ContatoContexto : DbContext
    {
        public ContatoContexto(DbContextOptions<ContatoContexto> options) : base(options)
        {
        }
        public DbSet<Importacao> Importacao { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Importacao>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
        }
    }
}