using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AbyKhedma.Entities;

namespace Infrastructure.Interceptors
{
    public class AuditableEntitiesInterceptor : SaveChangesInterceptor
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditableEntitiesInterceptor> _logger;

        public AuditableEntitiesInterceptor(
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditableEntitiesInterceptor> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            BeforeSaveTriggers(eventData.Context);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            BeforeSaveTriggers(eventData.Context);
            return ValueTask.FromResult(result);
        }

        private void BeforeSaveTriggers(DbContext? context)
        {
            var entries = context?.ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var identityClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "identityId");
                if (entityEntry.State == EntityState.Added)
                {
                    if(identityClaim != null)
                    {
                        ((BaseEntity)entityEntry.Entity).CreatedBy =int.Parse( identityClaim.Value);
                    }
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.UtcNow;
                }
                if (entityEntry.State == EntityState.Modified)
                {
                    ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.UtcNow;
                    if (identityClaim != null)
                    {
                        ((BaseEntity)entityEntry.Entity).UpdatedBy = int.Parse(identityClaim.Value);
                    }
                }
            }
        }
    }
}
