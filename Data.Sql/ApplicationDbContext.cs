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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
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
}