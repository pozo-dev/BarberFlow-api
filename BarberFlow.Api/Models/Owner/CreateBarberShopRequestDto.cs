using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Api.Models;
public class CreateBarberShopRequestDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;

    public int LocationSearchId { get; set; }
    [Required]
    public IFormFile Logo { get; set; } = null!;

    [Required]
    public IFormFile Banner { get; set; } = null!;
}
