using In.ProjectEKA.HipService.UserAuth.Model;
using Microsoft.EntityFrameworkCore;

namespace In.ProjectEKA.HipService.UserAuth.Database
{
    public class NdhmDemographicsContext : DbContext
    {
        public NdhmDemographicsContext(DbContextOptions<NdhmDemographicsContext> options) : base(options)
        {
        }

        public DbSet<NdhmDemographics> NdhmDemographics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NdhmDemographics>(builder =>
            {
                builder.HasKey(p=>p.HealthId);
                builder.Property(p => p.HealthId);
                builder.Property(p => p.Name);
                builder.Property(p => p.Gender);
                builder.Property(p => p.DateOfBirth);
                builder.Property(p => p.PhoneNumber);
            });
        }
    }
}