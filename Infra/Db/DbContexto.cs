using Microsoft.EntityFrameworkCore;
using minimal_api_DIO.Dominio.Entidades;

namespace minimal_api_DIO.Infra.Db;

public class DbContexto : DbContext
{
    public DbSet<Administrador> Administradores { get; set; } =  default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {  
                Id = 1,
                Email = "Adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            });
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Administrador.Sqlite");
        base.OnConfiguring(optionsBuilder);
    }
    
} 