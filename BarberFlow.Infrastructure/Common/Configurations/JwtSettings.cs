using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Infrastructure.Common.Configurations;
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Secret { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1, 1440)]
    public int ExpirationMinutes { get; set; }
}
