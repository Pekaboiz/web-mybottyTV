using Microsoft.EntityFrameworkCore;
using web_mybottyTV.Models;

namespace web_mybottyTV.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Channel>(e => 
            {
                e.ToTable("channels");

                e.HasIndex(c => c.ChannelName).IsUnique();
                e.Property(c => c.MaxWarns).HasDefaultValue(3);
                e.Property(c => c.BotEnabled).HasDefaultValue(true);

                e.HasMany(c => c.Commands)
                 .WithOne(cmd => cmd.Channel)
                 .HasForeignKey(cmd => cmd.ChannelId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Command>(e =>
            {
                e.ToTable("commands");

                e.Property(c => c.ActionType).HasConversion<string>();
                e.Property(c => c.AllowedRoles).HasConversion<string>();
            });
        }
    }
}
