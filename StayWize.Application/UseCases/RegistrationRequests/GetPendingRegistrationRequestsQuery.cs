using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.RegistrationRequests;

public record GetPendingRegistrationRequestsQuery : IRequest<IEnumerable<RegistrationRequestDto>>;

public class GetPendingRegistrationRequestsQueryHandler
    : IRequestHandler<GetPendingRegistrationRequestsQuery, IEnumerable<RegistrationRequestDto>>
{
    private readonly IClientRegistrationRequestRepository _repository;

    public GetPendingRegistrationRequestsQueryHandler(
        IClientRegistrationRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RegistrationRequestDto>> Handle(
        GetPendingRegistrationRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var requests = await _repository.GetPendingAsync();

        return requests.Select(r => new RegistrationRequestDto
        {
            Id             = r.Id,
            FirstName      = r.FirstName,
            LastName       = r.LastName,
            Email          = r.Email,
            DocumentNumber = r.DocumentNumber,
            Phone          = r.Phone,
            Status         = r.Status.ToString(),
            CreatedAt      = r.CreatedAt
        });
    }
}