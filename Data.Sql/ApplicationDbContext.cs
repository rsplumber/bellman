using Core.Domains.Jiring;
using Core.Domains.Pattern;
using Core.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Pattern> Patterns { get; set; }
    public DbSet<Jiring> Jirings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
        builder.ApplyConfiguration(new PatternEntityTypeConfiguration());
        builder.ApplyConfiguration(new JiringEntityTypeConfiguration());
        base.OnModelCreating(builder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    private class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications")
                .HasKey(notification => notification.Id);

            builder.Property(notification => notification.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(notification => notification.Content)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("content");
            
            builder.Property(notification => notification.Params)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("params")
                .IsRequired(false);
            
            builder.HasOne(model => model.Pattern)
                .WithMany()
                .HasForeignKey("pattern_id")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(notification => notification.From)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("from");

            builder.Property(notification => notification.To)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("to");

            builder.Property(notification => notification.Type)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("type");

            builder.Property(notification => notification.Retry)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("retry");

            builder.Property(notification => notification.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(notification => notification.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    } 
    
    
    private class PatternEntityTypeConfiguration : IEntityTypeConfiguration<Pattern>
    {
        public void Configure(EntityTypeBuilder<Pattern> builder)
        {
            builder.ToTable("patterns")
                .HasKey(pattern => pattern.Id);

            builder.Property(pattern => pattern.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(pattern => pattern.Template)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("template");
            
            builder.Property(pattern => pattern.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }
    private class JiringEntityTypeConfiguration : IEntityTypeConfiguration<Jiring>
    {
        public void Configure(EntityTypeBuilder<Jiring> builder)
        {
            builder.ToTable("jirings" , "jiring")
                .HasKey(pattern => pattern.Id);

            builder.Property(pattern => pattern.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(pattern => pattern.PatternId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("pattern_id");
            
            builder.Property(pattern => pattern.JiringId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("jiring_id");
        }
    }
}