namespace PeakLims.Databases;

using PeakLims.Domain;
using PeakLims.Databases.EntityConfigurations;
using PeakLims.Services;
using Configurations;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Resources;

public sealed class PeakLimsDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PeakLimsDbContext(
        DbContextOptions<PeakLimsDbContext> options, ICurrentUserService currentUserService, IMediator mediator, IDateTimeProvider dateTimeProvider) : base(options)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
        _dateTimeProvider = dateTimeProvider;
    }

    #region DbSet Region - Do Not Delete
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
        
        modelBuilder.HasSequence<long>(Consts.DatabaseSequences.PatientInternalIdPrefix)
            .StartsAt(10045702) // people don't like a nice round starting number
            .IncrementsBy(1);
        
        modelBuilder.HasSequence<long>(Consts.DatabaseSequences.AccessionNumberPrefix)
            .StartsAt(10005702) // people don't like a nice round starting number
            .IncrementsBy(1);
        
        modelBuilder.HasSequence<long>(Consts.DatabaseSequences.SampleNumberPrefix)
            .StartsAt(10000202) // people don't like a nice round starting number
            .IncrementsBy(1);

        modelBuilder.FilterSoftDeletedRecords();
        /* any query filters added after this will override soft delete 
                https://docs.microsoft.com/en-us/ef/core/querying/filters
                https://github.com/dotnet/efcore/issues/10275
        */

        #region Entity Database Config Region - Only delete if you don't want to automatically add configurations
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
                await _mediator.Publish(entityDomainEvent);
        }
    }
        
    private void UpdateAuditFields()
    {
        var now = _dateTimeProvider.DateTimeUtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.UpdateCreationProperties(now, _currentUserService?.UserId);
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    break;
                
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.UpdateModifiedProperties(now, _currentUserService?.UserId);
                    entry.Entity.UpdateIsDeleted(true);
                    break;
            }
        }
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
}
