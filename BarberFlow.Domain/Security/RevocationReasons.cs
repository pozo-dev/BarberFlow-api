namespace BarberFlow.Domain.Security
{
    public static class RevocationReasons
    {
        public const string Replaced = "Refresh token replaced";
        public const string ReuseDetected = "Refresh token reuse detected";
        public const string Logout = "User logged out";
    }
}
