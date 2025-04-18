using System;
using ManagementWeb.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagementWeb.Api.Contexts;

public class MyContext : DbContext
{
    public DbSet<TaskReport> Tasks { get; set; }
    public DbSet<RepairReport> RepairReports { get; set; }
    public DbSet<User> Users { get; set; }

    // config
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyAppTempDB;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True");
    }

    // model config
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
