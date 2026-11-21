using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

// Отдельный PATCH для назначения исполнителя (ЛР8: назначение и статус —
// два разных действия, один общий PATCH не используется).
public sealed class AssignTicketRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "assigneeUserId: целое число больше нуля")]
    public int AssigneeUserId { get; set; }
}
