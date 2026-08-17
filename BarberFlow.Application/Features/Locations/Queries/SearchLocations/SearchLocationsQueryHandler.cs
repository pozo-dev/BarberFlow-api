using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;
using System.Globalization;
using System.Text;

namespace BarberFlow.Application.Features.Locations.Queries.SearchLocations;

public sealed class SearchLocationsQueryHandler : IRequestHandler<SearchLocationsQuery, IReadOnlyList<LocationSearchDto>>
{
    private readonly ILocationSearchRepository _repository;
    public SearchLocationsQueryHandler(ILocationSearchRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<LocationSearchDto>> Handle(SearchLocationsQuery request, CancellationToken cancellationToken)
    {
        var search = Normalize(request.Search);
        if (search.Length < 2) return [];
        var locations = await _repository.SearchAsync(search, Math.Clamp(request.Take, 1, 30), cancellationToken);
        return locations.Select(x => new LocationSearchDto(x.Id, x.DisplayName)).ToList();
    }

    private static string Normalize(string value) => new string(
        value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
}
