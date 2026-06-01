using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Auth;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.RegistrationRequests;

public record ApproveRegistrationRequestCommand(Guid Id) : IRequest<InvitationResultDto>;

public class ApproveRegistrationRequestCommandHandler
    : IRequestHandler<ApproveRegistrationRequestCommand, InvitationResultDto>
{
    private readonly IClientRegistrationRequestRepository _repository;
    private readonly IMediator _mediator;

    public ApproveRegistrationRequestCommandHandler(
        IClientRegistrationRequestRepository repository,
        IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task<InvitationResultDto> Handle(
        ApproveRegistrationRequestCommand request,
        CancellationToken cancellationToken)
    {
        var registrationRequest = await _repository.GetByIdAsync(request.Id);

        if (registrationRequest is null)
            throw new NotFoundException("Solicitud de alta", request.Id);

        if (registrationRequest.Status != Domain.Entities.RegistrationRequestStatus.Pending)
            throw new ConflictException("La solicitud ya fue procesada.");

        // Crear invitación con rol Owner — envía email con one-time link
        var invitation = await _mediator.Send(new InviteUserCommand(new InviteUserDto
        {
            FirstName = registrationRequest.FirstName,
            LastName  = registrationRequest.LastName,
            Email     = registrationRequest.Email,
            Role      = "Owner"
        }), cancellationToken);

        // Marcar la solicitud como aprobada
        registrationRequest.Approve();
        await _repository.UpdateAsync(registrationRequest);

        return invitation;
    }
}