using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.Models;

namespace Web_Turismo_Triunvirato.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Usuarios { get; set; }

        // Puedes agregar más DbSet para otras entidades de tu base de datos
    }
}