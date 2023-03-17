using EMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.DL.DataContext;

public class EMSDbContext : DbContext
{
    public EMSDbContext(DbContextOptions<EMSDbContext> options) : base (options)
    {
        
    }

    public virtual DbSet<Employee>? Employee { get; set; }
    public virtual DbSet<Department>? Department { get; set; }
}