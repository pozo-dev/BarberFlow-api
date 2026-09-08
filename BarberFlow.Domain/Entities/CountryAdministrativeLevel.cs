namespace BarberFlow.Domain.Entities;
public class CountryAdministrativeLevel
{
    public int Id { get; private set; }
    public int CountryId { get; private set; }
    public int Level { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public Country Country { get; private set; } = null!;
    private CountryAdministrativeLevel() { }
}
