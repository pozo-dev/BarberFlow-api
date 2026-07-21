namespace BarberFlow.Domain.Entities
{
    public class Branch
    {
        public Guid Id { get; private set; }
        public Guid BarberShopId { get; private set; }

        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public BarberShop BarberShop { get; private set; } = null!;

        public ICollection<BarberAssignment> BarberAssignments { get; private set; } =
            new List<BarberAssignment>();

        private Branch() { }

        private Branch(
            Guid barberShopId,
            string name,
            string address,
            string phoneNumber)
        {
            Id = Guid.NewGuid();
            BarberShopId = barberShopId;
            Name = name.Trim();
            Address = address.Trim();
            PhoneNumber = phoneNumber.Trim();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static Branch Create(
            Guid barberShopId,
            string name,
            string address,
            string phoneNumber)
        {
            if (barberShopId == Guid.Empty)
                throw new ArgumentException("La barbería es requerida.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre de la sucursal es requerido.");

            if (name.Trim().Length > 120)
                throw new ArgumentException(
                    "El nombre no puede exceder 120 caracteres.");

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("La dirección es requerida.");

            if (address.Trim().Length > 300)
                throw new ArgumentException(
                    "La dirección no puede exceder 300 caracteres.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("El teléfono es requerido.");

            if (phoneNumber.Trim().Length > 20)
                throw new ArgumentException(
                    "El teléfono no puede exceder 20 caracteres.");

            return new Branch(
                barberShopId,
                name,
                address,
                phoneNumber);
        }

        public void Update(
            string name,
            string address,
            string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es requerido.");

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("La dirección es requerida.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("El teléfono es requerido.");

            Name = name.Trim();
            Address = address.Trim();
            PhoneNumber = phoneNumber.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}