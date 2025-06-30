using Microsoft.EntityFrameworkCore;
using ClienteAPI.Domain; // Para acessar as classes Cliente e Logradouro

namespace ClienteAPI.Data.Contexts
{
    /// Representa o contexto do banco de dados para as entidades de domínio (Clientes e Logradouros).

    public class ClienteDbContext : DbContext
    {
        public ClienteDbContext(DbContextOptions<ClienteDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Logradouro> Logradouros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garante que o endereço de e-mail do cliente seja único.
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Configura o relacionamento um-para-muitos entre Cliente e Logradouro.
            modelBuilder.Entity<Logradouro>()
                .HasOne(l => l.Cliente)
                .WithMany(c => c.Logradouros)
                .HasForeignKey(l => l.ClienteId);
        }
    }
}