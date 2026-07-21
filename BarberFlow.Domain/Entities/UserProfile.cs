namespace BarberFlow.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public int RoleId { get; private set; }
        public Guid? BarberShopId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; }
        public Role Role { get; private set; }
        public BarberShop? BarberShop { get; private set; }
        public ICollection<RefreshToken> RefreshTokens { get; private set; }
        public ICollection<BarberAssignment> BarberAssignments { get; private set; }

        private UserProfile()
        {
            RefreshTokens = new List<RefreshToken>();
            BarberAssignments = new List<BarberAssignment>();
        }

        public UserProfile(Guid userId, int roleId, Guid? barberShopId = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RoleId = roleId;
            BarberShopId = barberShopId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            RefreshTokens = new List<RefreshToken>();
            BarberAssignments = new List<BarberAssignment>();
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
