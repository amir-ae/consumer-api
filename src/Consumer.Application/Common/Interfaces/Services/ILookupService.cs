using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Services;

public interface ILookupService
{
    ValueTask<string?> CountryCode(CountryId countryId, CancellationToken ct = default);
    Task<PhoneNumber?> InspectCountryCode(PhoneNumber? phoneNumber, CancellationToken ct = default);
}