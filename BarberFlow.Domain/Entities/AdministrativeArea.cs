namespace BarberFlow.Domain.Entities;
public class AdministrativeArea
{
    public int Id { get; private set; }
    public int CountryAdministrativeLevelId { get; private set; }
    public int AdministrativeAreaTypeId { get; private set; }
    public int? ParentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public CountryAdministrativeLevel CountryAdministrativeLevel { get; private set; } = null!;
    public AdministrativeAreaType AdministrativeAreaType { get; private set; } = null!;
    public AdministrativeArea? Parent { get; private set; }
    public ICollection<AdministrativeArea> Children { get; private set; } = new List<AdministrativeArea>();
    private AdministrativeArea() { }
}
