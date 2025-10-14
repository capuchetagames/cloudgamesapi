using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    //private string _connectionString = "server=localhost;database=cloudgames;user=root;password=rooot123";
    private string _connectionString;

    public ApplicationDbContext()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        _connectionString = configuration.GetConnectionString("ConnectionString");
    }
    
    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Game> Games { get; set; }
    
    public DbSet<UserLibrary> UserLibrary { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured) return;

        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}