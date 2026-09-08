namespace BarberFlow.Domain.Entities;
public class Role
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private Role() { }

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
