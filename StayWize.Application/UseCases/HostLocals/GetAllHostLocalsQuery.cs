using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.HostLocals;

public record GetAllHostLocalsQuery : IRequest<IEnumerable<HostLocalDto>>;

public class GetAllHostLocalsQueryHandler
    : IRequestHandler<GetAllHostLocalsQuery, IEnumerable<HostLocalDto>>
{
    private readonly IHostLocalRepository _repository;

    public GetAllHostLocalsQueryHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HostLocalDto>> Handle(
        GetAllHostLocalsQuery request,
        CancellationToken cancellationToken)
    {
        var hostLocals = await _repository.GetAllAsync();

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