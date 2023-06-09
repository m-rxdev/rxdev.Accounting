using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using rxdev.Accounting.Model;

namespace rxdev.Accounting.Persistence;

public class AccountingDbContext : DbContext
{
    private class DecimalValueConverter
        : ValueConverter<decimal, int>
    {
        private const decimal Factor = 100; 
        public DecimalValueConverter()
            : base(v => (int)(v * Factor), v => v / Factor)
        { }
    }

    public AccountingDbContext()
    { }

    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options) 
    { }

    public override int SaveChanges()
    {
        var now = DateTime.UtcNow;

        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not Entity entity)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.EntityCreationDate = now;
                    entity.EntityEditionDate = now;
                    break;

                case EntityState.Modified:
                    entity.EntityEditionDate = now;
                    break;
            }
        }

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (Type type in EntityHelper.GetEntityTypes())
        {
            object entityBuilder = typeof(ModelBuilder).GetMethod("Entity", types: Type.EmptyTypes)!.MakeGenericMethod(type)!.Invoke(modelBuilder, null)!;
            type.GetMethod("CreateModel")?.Invoke(null, new object[] { entityBuilder });
        }

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HaveConversion<DecimalValueConverter>();
    }
}