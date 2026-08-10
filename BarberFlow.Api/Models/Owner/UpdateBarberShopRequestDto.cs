using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Api.Models
{
    public class UpdateBarberShopRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile Logo { get; set; } = null!;

        [Required]
        public IFormFile Banner { get; set; } = null!;
    }
}
