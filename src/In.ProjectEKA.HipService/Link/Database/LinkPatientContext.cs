namespace In.ProjectEKA.HipService.Link.Database
{
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class LinkPatientContext : DbContext
    {
        public LinkPatientContext(DbContextOptions<LinkPatientContext> options) : base(options)
        {
        }

        public DbSet<LinkRequest> LinkRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkedCareContext>()
                .HasKey("CareContextNumber", "LinkReferenceNumber")
                .HasName("Id");

            modelBuilder.Entity<LinkRequest>()
                .HasMany(l => l.CareContexts)
                .WithOne(c => c.LinkRequest)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}