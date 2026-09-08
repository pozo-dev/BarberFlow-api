namespace BarberFlow.Domain.Entities;
public class Country
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    private Country() { }
}
