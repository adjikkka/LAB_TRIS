namespace backend.Models;

// Главная сущность — обращение жильца (ЛР2, data-model.md).
public sealed class Ticket
{
    public int Id { get; set; }

    // Человеческий номер, например ОБР-2026-001; задаёт сервер
    public string Number { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = TicketStatuses.New;

    public int RoomId { get; set; }

    public Room? Room { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    // У нового обращения исполнитель может отсутствовать (NULL)
    public int? AssigneeUserId { get; set; }

    public User? Assignee { get; set; }
}
