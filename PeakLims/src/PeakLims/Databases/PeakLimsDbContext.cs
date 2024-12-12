namespace PeakLims.Databases;

using PeakLims.Domain;
using PeakLims.Databases.EntityConfigurations;
using PeakLims.Services;
using MediatR;
using PeakLims.Domain.RolePermissions;
using PeakLims.Domain.Users;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Containers;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Tests;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.PeakOrganizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.PatientRelationships;
using Exceptions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Resources;

public sealed class PeakLimsDbContext(
    DbContextOptions<PeakLimsDbContext> options,
    ICurrentUserService currentUserService,
    IMediator mediator,
    TimeProvider dateTimeProvider)
    : DbContext(options)
{
    #region DbSet Region - Do Not Delete
    public DbSet<PatientRelationship> PatientRelationships { get; set; }
    public DbSet<PeakOrganization> PeakOrganizations { get; set; }
    public DbSet<HipaaAuditLog> HipaaAuditLogs { get; set; }
    public DbSet<PanelOrder> PanelOrders { get; set; }
    public DbSet<AccessionAttachment> AccessionAttachments { get; set; }
    public DbSet<AccessionContact> AccessionContacts { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Accession> Accessions { get; set; }
    public DbSet<AccessionComment> AccessionComments { get; set; }
    public DbSet<Sample> Samples { get; set; }
    public DbSet<Container> Containers { get; set; }
    public DbSet<TestOrder> TestOrders { get; set; }
    public DbSet<Panel> Panels { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<HealthcareOrganization> HealthcareOrganizations { get; set; }
    public DbSet<HealthcareOrganizationContact> HealthcareOrganizationContacts { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    #endregion DbSet Region - Do Not Delete

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasSequence<int>(Consts.DatabaseSequences.PatientInternalIdPrefix)
            .StartsAt(100045702) // people don't like a nice round starting number
            .IncrementsBy(1);
        
        modelBuilder.HasSequence<int>(Consts.DatabaseSequences.AccessionNumberPrefix)
            .StartsAt(100005702) // people don't like a nice round starting number
            .IncrementsBy(1);
        
        modelBuilder.HasSequence<int>(Consts.DatabaseSequences.SampleNumberPrefix)
            .StartsAt(100000202) // people don't like a nice round starting number
            .IncrementsBy(1);

        modelBuilder.FilterSoftDeletedRecords();
        /* any query filters added after this will override soft delete 
                https://docs.microsoft.com/en-us/ef/core/querying/filters
                https://github.com/dotnet/efcore/issues/10275
        */

        #region Entity Database Config Region - Only delete if you don't want to automatically add configurations
        modelBuilder.ApplyConfiguration(new PatientRelationshipConfiguration());
        modelBuilder.ApplyConfiguration(new PanelTestAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new PeakOrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new HipaaAuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new PanelOrderConfiguration());
        modelBuilder.ApplyConfiguration(new AccessionAttachmentConfiguration());
        modelBuilder.ApplyConfiguration(new AccessionContactConfiguration());
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new AccessionConfiguration());
        modelBuilder.ApplyConfiguration(new AccessionCommentConfiguration());
        modelBuilder.ApplyConfiguration(new SampleConfiguration());
        modelBuilder.ApplyConfiguration(new ContainerConfiguration());
        modelBuilder.ApplyConfiguration(new TestOrderConfiguration());
        modelBuilder.ApplyConfiguration(new PanelConfiguration());
        modelBuilder.ApplyConfiguration(new TestConfiguration());
        modelBuilder.ApplyConfiguration(new HealthcareOrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new HealthcareOrganizationContactConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        #endregion Entity Database Config Region - Only delete if you don't want to automatically add configurations
    
        modelBuilder.Entity<Patient>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<Accession>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<Sample>()
            .HasQueryFilter(e => !e.IsDeleted && e.Patient.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<Container>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<HealthcareOrganization>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<HealthcareOrganizationContact>()
            .HasQueryFilter(e => !e.IsDeleted && e.HealthcareOrganization.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<AccessionComment>()
            .HasQueryFilter(e => !e.IsDeleted && e.Accession.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<AccessionContact>()
            .HasQueryFilter(e => !e.IsDeleted && e.Accession.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<AccessionAttachment>()
            .HasQueryFilter(e => !e.IsDeleted && e.Accession.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<HipaaAuditLog>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<TestOrder>()
            .HasQueryFilter(e => !e.IsDeleted && e.Accession.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<PanelOrder>()
            .HasQueryFilter(e => !e.IsDeleted && e.Panel.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<Test>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<Panel>()
            .HasQueryFilter(e => !e.IsDeleted && e.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<PanelTestAssignment>()
            .HasQueryFilter(e => !e.IsDeleted && e.Panel.OrganizationId == currentUserService.OrganizationId);
        modelBuilder.Entity<PatientRelationship>()
            .HasQueryFilter(e => !e.IsDeleted && e.FromPatient.OrganizationId == currentUserService.OrganizationId);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        var result = base.SaveChanges();
        _dispatchDomainEvents().GetAwaiter().GetResult();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        await _dispatchDomainEvents();
        return result;
    }
    
    private async Task _dispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<BaseEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var entityDomainEvent in events)
                await mediator.Publish(entityDomainEvent);
        }
    }
        
    private void UpdateAuditFields()
    {
        var now = dateTimeProvider.GetUtcNow();
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UpdateCreationProperties(now, currentUserService?.UserIdentifier);
                    entry.Entity.UpdateModifiedProperties(now, currentUserService?.UserIdentifier);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateModifiedProperties(now, currentUserService?.UserIdentifier);
                    break;
                
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.UpdateModifiedProperties(now, currentUserService?.UserIdentifier);
                    entry.Entity.UpdateIsDeleted(true);
                    break;
            }
        }
    }
    
    // due to dumb breaking change in .net 9... https://github.com/dotnet/efcore/issues/34431
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

    }
}

public static class Extensions
{
    public static void FilterSoftDeletedRecords(this ModelBuilder modelBuilder)
    {
        Expression<Func<BaseEntity, bool>> filterExpr = e => !e.IsDeleted;
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes()
            .Where(m => m.ClrType.IsAssignableTo(typeof(BaseEntity))))
        {
            // modify expression to handle correct child type
            var parameter = Expression.Parameter(mutableEntityType.ClrType);
            var body = ReplacingExpressionVisitor
                .Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
            var lambdaExpression = Expression.Lambda(body, parameter);

            // set filter
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }
    
    public static async Task<TEntity> GetByIdOrDefault<TEntity> (this DbSet<TEntity> dbSet, 
        Guid id, 
        CancellationToken cancellationToken = default) 
            where TEntity : BaseEntity
    {
        return await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    public static async Task<TEntity> GetByIdOrDefault<TEntity> (this IQueryable<TEntity> query, 
        Guid id, 
        CancellationToken cancellationToken = default) 
            where TEntity : BaseEntity
    {
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    } 
    
    public static async Task<TEntity> GetById<TEntity> (this DbSet<TEntity> dbSet, 
        Guid id, 
        CancellationToken cancellationToken = default) 
            where TEntity : BaseEntity
    {
        var result = await dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return result.MustBeFoundOrThrow();
    }
    
    public static async Task<TEntity> GetById<TEntity> (this IQueryable<TEntity> query, 
        Guid id, 
        CancellationToken cancellationToken = default) 
            where TEntity : BaseEntity
    {
        var result = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return result.MustBeFoundOrThrow();
    }

    public static IQueryable<User> GetUserAggregate(this PeakLimsDbContext dbContext)
    {
        return dbContext.Users
            .Include(u => u.Roles);
    }

    public static IQueryable<Accession> GetAccessionAggregate(this PeakLimsDbContext dbContext)
    {
        return dbContext.Accessions
            .Include(x => x.Patient)
                // .ThenInclude(x => x.Samples)
            .Include(x => x.HealthcareOrganization)
            .Include(x => x.AccessionContacts)
                .ThenInclude(x => x.HealthcareOrganizationContact)
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.Test)
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.Sample)
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.PanelOrder)
                .ThenInclude(x => x.Panel)
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.Sample)
            .Include(x => x.AccessionContacts)
            .Include(x => x.Patient)
                .ThenInclude(x => x.FromRelationships)
            .Include(x => x.Patient)
                .ThenInclude(x => x.ToRelationships)
            .AsSplitQuery();
    }

    public static IQueryable<Patient> GetPatientAggregate(this PeakLimsDbContext dbContext)
    {
        return dbContext.Patients
            // .Include(x => x.Samples)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.HealthcareOrganization)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.AccessionContacts)
                .ThenInclude(x => x.HealthcareOrganizationContact)
                .ThenInclude(x => x.HealthcareOrganization)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.TestOrders)
                .ThenInclude(x => x.Test)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.TestOrders)
                .ThenInclude(x => x.Sample)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.TestOrders)
                .ThenInclude(x => x.Test)
            .Include(x => x.Accessions)
                .ThenInclude(x => x.TestOrders)
                .ThenInclude(x => x.PanelOrder)
                .ThenInclude(x => x.Panel)
            .Include(x => x.FromRelationships)
            .Include(x => x.ToRelationships)
            .AsSplitQuery();
    }
    
    public static TEntity MustBeFoundOrThrow<TEntity>(this TEntity entity)
        where TEntity : BaseEntity
    {
         return entity ?? throw new NotFoundException($"{typeof(TEntity).Name} was not found.");
    }
}
