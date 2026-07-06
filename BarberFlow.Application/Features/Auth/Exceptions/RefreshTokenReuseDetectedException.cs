namespace BarberFlow.Application.Features.Auth.Exceptions
{
    public sealed class RefreshTokenReuseDetectedException : Exception
    {
        public RefreshTokenReuseDetectedException()
            : base("A revoked refresh token was reused. All active sessions have been revoked. Please sign in again.")
        {
        }
    }
}
