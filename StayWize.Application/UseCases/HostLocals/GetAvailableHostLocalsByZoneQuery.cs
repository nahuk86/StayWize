using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.HostLocals;

public record GetAvailableHostLocalsByZoneQuery(string Zone) : IRequest<IEnumerable<HostLocalDto>>;

public class GetAvailableHostLocalsByZoneQueryHandler
    : IRequestHandler<GetAvailableHostLocalsByZoneQuery, IEnumerable<HostLocalDto>>
{
    private readonly IHostLocalRepository _repository;

    public GetAvailableHostLocalsByZoneQueryHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HostLocalDto>> Handle(
        GetAvailableHostLocalsByZoneQuery request,
        CancellationToken cancellationToken)
    {
        var hostLocals = await _repository.GetAvailableByZoneAsync(request.Zone);

        return hostLocals.Select(h => new HostLocalDto
        {
            Id = h.Id,
            FirstName = h.FirstName,
            LastName = h.LastName,
            Email = h.Email,
            Phone = h.Phone,
            Zone = h.Zone,
            IsAvailable = h.IsAvailable,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        });
    }
}