using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wize.common.tenancy.Interfaces;
using wize.settings.data.V1.Models;

namespace wize.settings.data.V1
{
    public class WizeContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public WizeContext(DbContextOptions<WizeContext> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
            //this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = AddTenancy(modelBuilder);
            //modelBuilder.Entity<WizeSetting>().Property(typeof(Guid), "TenantId");
            modelBuilder.Entity<WizeSetting>().HasKey(new string[] { "Name", "Type", "TenantId" });
            base.OnModelCreating(modelBuilder);
        }
        public override TEntity Find<TEntity>(params object[] keyValues)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.Find<TEntity>(keyValues);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            if (!tenantId.HasValue || modelTenantId != tenantId.Value)
                return default;

            return model;
        }

        public override object Find(Type entityType, params object[] keyValues)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.Find(entityType, keyValues);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            //if (!tenantId.HasValue || modelTenantId != tenantId.Value)
            //    return default;

            return model;
        }

        public override ValueTask<object> FindAsync(Type entityType, params object[] keyValues)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.FindAsync(entityType, keyValues);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            //if (!tenantId.HasValue || modelTenantId != tenantId.Value)
            //    return default;

            return model;
        }

        public override ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.FindAsync<TEntity>(keyValues);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            //if (!tenantId.HasValue || modelTenantId != tenantId.Value)
            //    return default;

            return model;
        }

        public override ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.FindAsync<TEntity>(keyValues, cancellationToken);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            //if (!tenantId.HasValue || modelTenantId != tenantId.Value)
            //    return default;

            return model;
        }

        public override ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            keyValues.Append(tenantId);
            var keys = keyValues.ToList();
            keys.Add(tenantId);
            keyValues = keys.ToArray();
            var model = base.FindAsync(entityType, keyValues, cancellationToken);
            var modelTenantId = base.Entry(model).CurrentValues.GetValue<Guid>("TenantId");
            //if (!tenantId.HasValue || modelTenantId != tenantId.Value)
            //    return default;

            return model;
        }

        public override int SaveChanges()
        {
            ApplyTenancy();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyTenancy();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTenancy();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyTenancy();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyTenancy()
        {
            var modified = ChangeTracker.Entries<ITenantModel>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entity in modified)
            {
                var property = entity.Property("TenantId");
                if (property != null)
                {
                    property.OriginalValue = property.CurrentValue = _tenantProvider.GetTenantId();
                    //property.IsModified = false;
                }
            }
        }

        private ModelBuilder AddTenancy(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WizeSetting>().Property<Guid>("TenantId");

            return modelBuilder;
        }


        public virtual DbSet<WizeSetting> WizeSettings { get; set; }
    }
}
