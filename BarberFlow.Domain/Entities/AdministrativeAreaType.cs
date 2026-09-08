namespace BarberFlow.Domain.Entities;
public class AdministrativeAreaType
{
    public int Id { get; private set; }
    public int CountryAdministrativeLevelId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public CountryAdministrativeLevel CountryAdministrativeLevel { get; private set; } = null!;
    private AdministrativeAreaType() { }
}
