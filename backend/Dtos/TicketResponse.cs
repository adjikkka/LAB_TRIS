namespace backend.Dtos;

// Ответ API по контракту из docs/api-contract.md
// (дополнено createdAt из ТЗ ЛР1: «дату создания»).
public sealed class TicketResponse
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int RoomId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public int? AssigneeUserId { get; set; }
}
