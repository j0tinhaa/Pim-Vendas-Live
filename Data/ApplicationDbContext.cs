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
        public DbSet<ProdutoModel> Produtos { get; set; } = null!;
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

            // Venda → Produto (muitos para um, opcional)
            modelBuilder.Entity<VendaModel>()
                .HasOne(v => v.Produto)
                .WithMany(p => p.Vendas)
                .HasForeignKey(v => v.ProdutoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índice único: código do produto
            modelBuilder.Entity<ProdutoModel>()
                .HasIndex(p => p.Codigo)
                .IsUnique();
        }
    }
}
