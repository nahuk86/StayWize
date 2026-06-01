using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.RegistrationRequests;

public record CreateRegistrationRequestCommand(CreateRegistrationRequestDto Dto) : IRequest<RegistrationRequestDto>;

public class CreateRegistrationRequestCommandHandler
    : IRequestHandler<CreateRegistrationRequestCommand, RegistrationRequestDto>
{
    private readonly IClientRegistrationRequestRepository _repository;
    private readonly IEmailService _emailService;

    public CreateRegistrationRequestCommandHandler(
        IClientRegistrationRequestRepository repository,
        IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task<RegistrationRequestDto> Handle(
        CreateRegistrationRequestCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Validar que no haya una solicitud pendiente con el mismo email o documento
        if (await _repository.ExistsByEmailAsync(dto.Email))
            throw new ConflictException($"Ya existe una solicitud pendiente para el email {dto.Email}.");

        if (await _repository.ExistsByDocumentNumberAsync(dto.DocumentNumber))
            throw new ConflictException($"Ya existe una solicitud pendiente para el documento {dto.DocumentNumber}.");

        var registrationRequest = ClientRegistrationRequest.Create(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.DocumentNumber,
            dto.Phone);

        await _repository.AddAsync(registrationRequest);

        // Notificar al equipo — fire-and-forget
        _ = _emailService.SendNewRegistrationRequestAsync(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.DocumentNumber,
            dto.Phone);

        return new RegistrationRequestDto
        {
            Id             = registrationRequest.Id,
            FirstName      = registrationRequest.FirstName,
            LastName       = registrationRequest.LastName,
            Email          = registrationRequest.Email,
            DocumentNumber = registrationRequest.DocumentNumber,
            Phone          = registrationRequest.Phone,
            Status         = registrationRequest.Status.ToString(),
            CreatedAt      = registrationRequest.CreatedAt
        };
    }
}