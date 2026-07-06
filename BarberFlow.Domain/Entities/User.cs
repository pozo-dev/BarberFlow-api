namespace BarberFlow.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<UserProfile> Profiles { get; private set; }
        public ICollection<Appointment> Appointments { get; private set; }
        public ICollection<RefreshToken> RefreshTokens { get; private set; }
        public ICollection<OtpCode> OtpCodes { get; private set; }

        private User()
        {
            Profiles = new List<UserProfile>();
            Appointments = new List<Appointment>();
            RefreshTokens = new List<RefreshToken>();
            OtpCodes = new List<OtpCode>();
        }

        public User(string phoneNumber)
        {
            Id = Guid.NewGuid();
            PhoneNumber = phoneNumber;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Profiles = new List<UserProfile>();
            Appointments = new List<Appointment>();
            RefreshTokens = new List<RefreshToken>();
            OtpCodes = new List<OtpCode>();
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
