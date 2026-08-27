namespace BarberFlow.Domain.Entities
{
    public class OtpCode
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string CodeHash { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public int FailedAttempts { get; private set; }
        public Guid? RequestedUserProfileId { get; private set; }
        public User User { get; private set; }

        private OtpCode() { }

        public OtpCode(Guid userId, Guid? userProfileId, string codeHash, DateTimeOffset expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RequestedUserProfileId = userProfileId;
            CodeHash = codeHash;
            ExpiresAt = expiresAt;
            CreatedAt = DateTimeOffset.UtcNow;
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public bool IsValid(string codeHash, int maxAttempts)
        {
            return !IsUsed && !IsBlocked(maxAttempts) && CodeHash == codeHash && ExpiresAt > DateTimeOffset.UtcNow;
        }

        public void IncrementFailedAttempts() => FailedAttempts++;

        public bool IsBlocked(int maxAttempts) => FailedAttempts >= maxAttempts;
    }
}
