using Core.Notifications;
using Core.Providers;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Sql;

public class NotificationCenterDbContext : DbContext
{
    private readonly ICapPublisher _eventBus;

    public NotificationCenterDbContext(DbContextOptions<NotificationCenterDbContext> options, ICapPublisher eventBus) : base(options)
    {
        _eventBus = eventBus;
    }

    public DbSet<Provider> Providers { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProviderEntityTypeConfiguration());
        builder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
        base.OnModelCreating(builder);
    }

    private class ProviderEntityTypeConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.ToTable("providers")
                .HasKey(provider => provider.Id);

            builder.Property(provider => provider.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(provider => provider.Name)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("name");

            builder.Property(provider => provider.Type)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("type");

            builder.Property(provider => provider.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(provider => provider.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");

            builder.Property(provider => provider.Metas)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("metas");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
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

            builder.Property(notification => notification.ProviderId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("provider_id");

            builder.Property(notification => notification.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(notification => notification.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }
}