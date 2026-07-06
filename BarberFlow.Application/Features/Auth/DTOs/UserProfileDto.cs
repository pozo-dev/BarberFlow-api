namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }          // ProfileId (clave para switch)
        public int RoleId { get; set; }       // Client / Barber
        public string RoleName { get; set; }  // "Client", "Barber"

        // 👇 Solo si es barbero
        public Guid? BarberShopId { get; set; }
        public string? BarberShopName { get; set; }
    }
}
