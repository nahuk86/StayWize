using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.HostLocals;

public record GetHostLocalByIdQuery(Guid Id) : IRequest<HostLocalDto?>;

public class GetHostLocalByIdQueryHandler
    : IRequestHandler<GetHostLocalByIdQuery, HostLocalDto?>
{
    private readonly IHostLocalRepository _repository;

    public GetHostLocalByIdQueryHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<HostLocalDto?> Handle(
        GetHostLocalByIdQuery request,
        CancellationToken cancellationToken)
    {
        var hostLocal = await _repository.GetByIdAsync(request.Id);

        if (hostLocal is null) return null;

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