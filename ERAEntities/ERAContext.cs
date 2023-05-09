using Microsoft.EntityFrameworkCore;
using ERAEntities.DataModels;
using Microsoft.Extensions.Configuration;

namespace ERAEntities
{
    public class ERAContext: DbContext
    {
        public ERAContext(DbContextOptions<ERAContext> options) : base(options)
        {
        }

        public virtual DbSet<Assistant> Assistants { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<AssistantLocation> AssistantGeoLocations { get; set; }
        public virtual DbSet<CustomerAssistantAssignment> CustomerAssistantAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Assistant>().ToTable("Assistants");
            builder.Entity<Customer>().ToTable("Customers");
            builder.Entity<AssistantLocation>().ToTable("AssistantLocations");
            builder.Entity<CustomerAssistantAssignment>().ToTable("CustomerAssistantAssignments");
        }
       
    }
}