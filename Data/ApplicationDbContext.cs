using LiveStore.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<LiveModel> Lives { get; set; } = null!;
        public DbSet<VendaModel> Vendas { get; set; } = null!;
        public DbSet<ClienteModel> Clientes { get; set; } = null!;
        public DbSet<GastoMensalModel> Gastos { get; set; } = null!;
        public DbSet<ClienteLiveRelatorioModel> RelatoriosEnviados { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // RelatorioEnviado — chave primária composta
            modelBuilder.Entity<ClienteLiveRelatorioModel>()
                .HasKey(r => new { r.LiveId, r.ClienteInstagram });

            modelBuilder.Entity<ClienteLiveRelatorioModel>()
                .HasOne(r => r.Live)
                .WithMany()
                .HasForeignKey(r => r.LiveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClienteLiveRelatorioModel>()
                .HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.ClienteInstagram)
                .OnDelete(DeleteBehavior.Cascade);

            // Cliente — chave primária é string (InstagramUser)
            modelBuilder.Entity<ClienteModel>()
                .HasKey(c => c.InstagramUser);

            // Venda → Cliente (muitos para um)
            modelBuilder.Entity<VendaModel>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteInstagram)
                .OnDelete(DeleteBehavior.Restrict);

            // Venda → Live (muitos para um)
            modelBuilder.Entity<VendaModel>()
                .HasOne(v => v.Live)
                .WithMany(l => l.Vendas)
                .HasForeignKey(v => v.LiveId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unicidade: (LiveId, Codigo) na Venda
            modelBuilder.Entity<VendaModel>()
                .HasIndex(v => new { v.LiveId, v.Codigo })
                .IsUnique();
        }
    }
}
