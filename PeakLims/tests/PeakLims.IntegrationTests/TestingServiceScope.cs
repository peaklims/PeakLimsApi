namespace PeakLims.IntegrationTests;

using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.S3;
using Databases;
using Domain.PeakOrganizations;
using Domain.PeakOrganizations.Models;
using Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using static TestFixture;
using HeimGuard;
using Microsoft.AspNetCore.Http;
using NSubstitute.ExceptionExtensions;
using SharedTestHelpers.Fakes.ClaimsPrincipal;
using SharedTestHelpers.Fakes.PeakOrganization;
using SharedTestHelpers.Utilities;

public class TestingServiceScope 
{
    private readonly IServiceScope _scope;

    public TestingServiceScope()
    {
        _scope = BaseScopeFactory.CreateScope();
        
        var userClaim = new FakeClaimsPrincipalBuilder().Build();
        SetUser(userClaim);
        
        SetUserIsPermitted();
    }

    public TScopedService GetService<TScopedService>()
    {
        var service = _scope.ServiceProvider.GetService<TScopedService>();
        return service;
    }

    public void SetUser(ClaimsPrincipal user)
    {
        var httpContextAccessor = BaseScopeFactory.CreateScope()
            .ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = user
        };
    }

    public void SetRandomUserInNewOrg()
    {
        var userClaim = new FakeClaimsPrincipalBuilder()
            .WithOrganizationId(Guid.NewGuid())
            .Build();
        SetUser(userClaim);
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

    public void Add<TEntity>(TEntity entity)
        where TEntity : class
    {
        var context = _scope.ServiceProvider.GetService<PeakLimsDbContext>();
        context.Add(entity);
        context.SaveChanges();
    }
    
    public async Task<bool> FileExistsInS3Async(string bucketName, string key)
    {
        var s3Client = _scope.ServiceProvider.GetService<IAmazonS3>();
        try
        {
            await s3Client.GetObjectMetadataAsync(bucketName, key);
            return true;
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            throw;
        }
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
        userPolicyHandler.MustHavePermission<ForbiddenAccessException>(permission)
            .ThrowsAsync(new ForbiddenAccessException());
        userPolicyHandler.HasPermissionAsync(permission)
            .Returns(false);
    }

    public void SetUserIsPermitted()
    {
        var userPolicyHandler = GetService<IHeimGuardClient>();
        userPolicyHandler.MustHavePermission<ForbiddenAccessException>(Arg.Any<string>())
            .Returns(Task.CompletedTask);
        userPolicyHandler.HasPermissionAsync(Arg.Any<string>())
            .Returns(true);
    }
}