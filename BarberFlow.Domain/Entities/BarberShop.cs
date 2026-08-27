namespace BarberFlow.Domain.Entities
{
    public class BarberShop
    {
        public Guid Id { get; private set; }

        public Guid OwnerUserId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public byte[] Logo { get; private set; } = Array.Empty<byte>();

        public byte[] Banner { get; private set; } = Array.Empty<byte>();

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public ICollection<Service> Services { get; private set; }

        public ICollection<UserProfile> Barbers { get; private set; }

        public ICollection<Branch> Branches { get; private set; }

        private BarberShop()
        {
            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Branches = new List<Branch>();
        }

        private BarberShop(
            Guid ownerUserId,
            string name,
            string description,
            byte[] logo,
            byte[] banner)
        {
            Id = Guid.NewGuid();

            OwnerUserId = ownerUserId;

            Name = name.Trim();

            Description = description.Trim();

            Logo = logo;

            Banner = banner;

            IsActive = true;

            CreatedAt = DateTimeOffset.UtcNow;

            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Branches = new List<Branch>();
        }

        public static BarberShop Create(
            Guid ownerUserId,
            string name,
            string description,
            byte[] logo,
            byte[] banner)
        {
            return new BarberShop(ownerUserId, name, description, logo, banner);
        }

        public void Update(
            string name,
            string description,
            byte[] logo,
            byte[] banner)
        {
            Name = name.Trim();

            Description = description.Trim();

            Logo = logo;

            Banner = banner;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
