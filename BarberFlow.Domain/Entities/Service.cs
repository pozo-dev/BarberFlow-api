namespace BarberFlow.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; private set; }
        public Guid BarberShopId { get; private set; }

        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        public decimal Price { get; private set; }
        public TimeSpan Duration { get; private set; }

        public bool IsActive { get; private set; }
        public int DisplayOrder { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public BarberShop BarberShop { get; private set; } = null!;

        private Service() { }

        private Service(
            Guid barberShopId,
            string name,
            decimal price,
            TimeSpan duration,
            string? description,
            int displayOrder)
        {
            Id = Guid.NewGuid();
            BarberShopId = barberShopId;
            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            Duration = duration;
            DisplayOrder = displayOrder;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static Service Create(
            Guid barberShopId,
            string name,
            decimal price,
            TimeSpan duration,
            string? description,
            int displayOrder = 0)
        {
            if (barberShopId == Guid.Empty)
                throw new ArgumentException("La barbería es requerida.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es requerido.");

            if (name.Trim().Length > 80)
                throw new ArgumentException("El nombre no puede exceder 80 caracteres.");

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            if (duration.TotalMinutes < 5)
                throw new ArgumentException("La duración mínima es 5 minutos.");

            if (duration.TotalHours > 8)
                throw new ArgumentException("La duración máxima es 8 horas.");

            return new Service(
                barberShopId,
                name,
                price,
                duration,
                description,
                displayOrder);
        }

        public void Update(
            string name,
            decimal price,
            TimeSpan duration,
            string? description,
            int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es requerido.");

            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            if (duration.TotalMinutes < 5)
                throw new ArgumentException("La duración mínima es 5 minutos.");

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            Duration = duration;
            DisplayOrder = displayOrder;
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
