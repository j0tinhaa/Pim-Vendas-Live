using Microsoft.EntityFrameworkCore;
using PimVendas.Models;

namespace PimVendas.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<EmprestimosModel> Emprestimos { get; set; }

    }
}
