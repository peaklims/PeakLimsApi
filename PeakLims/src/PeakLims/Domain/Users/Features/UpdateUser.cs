namespace PeakLims.Domain.Users.Features;

using PeakLims.Domain.Users;
using PeakLims.Domain.Users.Dtos;
using PeakLims.Domain.Users.Services;
using PeakLims.Services;
using PeakLims.Domain.Users.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateUser
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly UserForUpdateDto UpdatedUserData;

        public Command(Guid id, UserForUpdateDto updatedUserData)
        {
            Id = id;
            UpdatedUserData = updatedUserData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IUserRepository userRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateUsers);

            var userToUpdate = await _userRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var userToAdd = request.UpdatedUserData.ToUserForUpdate();
            userToUpdate.Update(userToAdd);

            _userRepository.Update(userToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}