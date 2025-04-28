using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Brain_Tumor_Classification.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Tumor>Tumors { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<PredictionResult> PredictionResults { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define roles
        var roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {   
                Name = "User",
                NormalizedName = "USER"
            }
        };

        // Seed roles
        builder.Entity<IdentityRole>().HasData(roles);
    }

}
