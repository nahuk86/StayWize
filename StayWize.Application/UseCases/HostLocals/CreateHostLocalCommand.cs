using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;

namespace StayWize.Application.UseCases.HostLocals;

public record CreateHostLocalCommand(CreateHostLocalDto Dto) : IRequest<HostLocalDto>;

public class CreateHostLocalCommandHandler
    : IRequestHandler<CreateHostLocalCommand, HostLocalDto>
{
    private readonly IHostLocalRepository _repository;

    public CreateHostLocalCommandHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<HostLocalDto> Handle(
        CreateHostLocalCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existing = await _repository.GetAllAsync();
        if (existing.Any(h => h.Email == dto.Email))
            throw new InvalidOperationException($"Ya existe un host local con el email {dto.Email}.");

        var hostLocal = HostLocal.Create(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Phone,
            dto.Zone);

        await _repository.AddAsync(hostLocal);

        return new HostLocalDto
        {
            Id = hostLocal.Id,
            FirstName = hostLocal.FirstName,
            LastName = hostLocal.LastName,
            Email = hostLocal.Email,
            Phone = hostLocal.Phone,
            Zone = hostLocal.Zone,
            IsAvailable = hostLocal.IsAvailable,
            CreatedAt = hostLocal.CreatedAt,
            UpdatedAt = hostLocal.UpdatedAt
        };
    }
}