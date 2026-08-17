namespace BarberFlow.Domain.Entities
{
    public class LocationSearch
    {
        public int Id { get; private set; }
        public int AdministrativeAreaId { get; private set; }
        public string DisplayName { get; private set; } = string.Empty;
        public string SearchText { get; private set; } = string.Empty;
        public AdministrativeArea AdministrativeArea { get; private set; } = null!;
        private LocationSearch() { }
    }
}
