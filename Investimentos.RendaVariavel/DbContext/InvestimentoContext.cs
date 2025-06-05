using Microsoft.EntityFrameworkCore;
using InvestimentosRendaVariavel.Models;

namespace InvestimentosRendaVariavel.DbContexto


{
    public class InvestimentoContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ativo> Ativos { get; set; }
        public DbSet<Operacao> Operacoes { get; set; }
        public DbSet<Cotacao> Cotacoes { get; set; }
        public DbSet<Posicao> Posicoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql("server=localhost;database=investimentos_db;user=root;password=1234",
                new MySqlServerVersion(new Version(8, 0, 25)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("usuarios").HasKey(u => u.Id);
            modelBuilder.Entity<Ativo>().ToTable("ativos").HasKey(a => a.Id);
            modelBuilder.Entity<Operacao>().ToTable("operacoes").HasKey(o => o.Id);
            modelBuilder.Entity<Cotacao>().ToTable("cotacoes").HasKey(c => c.Id);
            modelBuilder.Entity<Posicao>().ToTable("posicoes").HasKey(p => p.Id);
        }
    }
}