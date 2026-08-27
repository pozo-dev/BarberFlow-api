namespace BarberFlow.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid UserProfileId { get; private set; }

        public string Token { get; private set; } = null!;

        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? RevokedAt { get; private set; }
        public string? RevocationReason { get; private set; }

        public string DeviceId { get; private set; } = null!;

        public bool IsRevoked => RevokedAt.HasValue;

        // Navegación (opcional)
        public User User { get; private set; } = null!;
        public UserProfile UserProfile { get; private set; } = null!;

        private RefreshToken() { } // EF Core

        // 🔒 Constructor privado
        private RefreshToken(Guid userId, Guid userProfileId, string token, DateTimeOffset expiresAt, string deviceId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UserProfileId = userProfileId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTimeOffset.UtcNow;
            DeviceId = deviceId;
        }

        // 🏭 FACTORY METHOD
        public static RefreshToken Create(Guid userId, Guid userProfileId, string token, DateTimeOffset expiresAt, string deviceId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.");

            if (userProfileId == Guid.Empty)
                throw new ArgumentException("UserProfileId is required.");

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required.");

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("DeviceId is required.");

            if (expiresAt <= DateTimeOffset.UtcNow)
                throw new ArgumentException("Expiration must be in the future.");

            return new RefreshToken(userId, userProfileId, token, expiresAt, deviceId);
        }

        // 🔥 Estado del token
        public bool IsActive()
        {
            return !IsRevoked && !IsExpired();
        }

        public bool IsExpired()
        {
            return DateTimeOffset.UtcNow >= ExpiresAt;
        }

        // 🔐 Revocar token (con trazabilidad)
        public void Revoke(string? reason = null)
        {
            if (IsRevoked)
                return;

            RevokedAt = DateTimeOffset.UtcNow;
            RevocationReason = reason;
        }
    }
}
