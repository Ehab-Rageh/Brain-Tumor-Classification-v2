using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Brain_Tumor_Classification.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Tumor>Tumors { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
}
