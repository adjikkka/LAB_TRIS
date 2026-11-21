using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

// Входной DTO создания (ЛР8): клиент присылает только поля, которые
// заполняет человек. id, number, status сервер задаёт сам — их здесь нет.
public sealed class CreateTicketRequest
{
    [Required(ErrorMessage = "Тема обязательна")]
    [StringLength(80, MinimumLength = 5, ErrorMessage = "Тема: от 5 до 80 символов")]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Комната: целое число больше нуля")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Описание обязательно")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Описание: от 10 до 500 символов")]
    public string Description { get; set; } = string.Empty;
}
