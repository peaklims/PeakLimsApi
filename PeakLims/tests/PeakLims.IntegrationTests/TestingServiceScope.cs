namespace PeakLims.IntegrationTests;

using System.Threading.Tasks;
using Databases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SharedKernel.Exceptions;
using static TestFixture;
using HeimGuard;

public class TestingServiceScope 
{
    private readonly IServiceScope _scope;

    public TestingServiceScope()
    {
        _scope = BaseScopeFactory.CreateScope();
        SetUserIsPermitted();
    }

    public TScopedService GetService<TScopedService>()
    {
        var service = _scope.ServiceProvider.GetService<TScopedService>();
        return service;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        var mediator = _scope.ServiceProvider.GetService<ISender>();
        return await mediator.Send(request);
    }

    public async Task SendAsync<TRequest>(TRequest request)
        where TRequest : IRequest
    {
        var mediator = _scope.ServiceProvider.GetService<ISender>();
        await mediator.Send(request);
    }

    public async Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        var context = _scope.ServiceProvider.GetService<PeakLimsDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        var context = _scope.ServiceProvider.GetService<PeakLimsDbContext>();
        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        var dbContext = _scope.ServiceProvider.GetRequiredService<PeakLimsDbContext>();

        try
        {
            //await dbContext.BeginTransactionAsync();

            var result = await action(_scope.ServiceProvider);

            //await dbContext.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            //dbContext.RollbackTransaction();
            throw;
        }
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<PeakLimsDbContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<PeakLimsDbContext>()));
    
    public Task<int> InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }
            return db.SaveChangesAsync();
        });
    }

    public void SetUserNotPermitted(string permission)
    {
        var userPolicyHandler = GetService<IHeimGuardClient>();
        Mock.Get(userPolicyHandler)
            .Setup(x => x.MustHavePermission<ForbiddenAccessException>(permission))
            .ThrowsAsync(new ForbiddenAccessException());
        Mock.Get(userPolicyHandler)
            .Setup(x => x.HasPermissionAsync(permission))
            .ReturnsAsync(false);
    }

    public void SetUserIsPermitted()
    {
        var userPolicyHandler = GetService<IHeimGuardClient>();
        Mock.Get(userPolicyHandler)
            .Setup(x => x.MustHavePermission<ForbiddenAccessException>(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        Mock.Get(userPolicyHandler)
            .Setup(x => x.HasPermissionAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
    }
}