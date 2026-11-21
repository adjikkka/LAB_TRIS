using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

// Отдельный PATCH для смены статуса.
public sealed class UpdateTicketStatusRequest
{
    [Required(ErrorMessage = "Статус обязателен")]
    public string Status { get; set; } = string.Empty;
}
