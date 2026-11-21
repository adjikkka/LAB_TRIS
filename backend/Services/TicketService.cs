using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Dtos;
using backend.Models;

namespace backend.Services;

// Сервис хранит правила процесса (ЛР8): проверка полей, номера, статусы.
// Хранение — PostgreSQL через EF Core (ЛР10), источника данных в памяти больше нет.
public sealed class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult<IReadOnlyList<TicketResponse>>> GetTicketsAsync(int userId, string role, CancellationToken cancellationToken = default)
    {
        // По ТЗ ЛР1: комендант видит все обращения, исполнитель — только свои
        var query = _db.Tickets.AsQueryable();
        if (!TicketRoles.IsCommandant(role))
        {
            query = query.Where(t => t.AssigneeUserId == userId);
        }

        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<TicketResponse>>.Ok(
            tickets.Select(MapToResponse).ToList());
    }

    public async Task<ServiceResult<TicketResponse>> GetTicketAsync(int id, int userId, string role, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<TicketResponse>.Fail(404, "Обращение не найдено");
        }

        if (!TicketRoles.IsCommandant(role) && ticket.AssigneeUserId != userId)
        {
            return ServiceResult<TicketResponse>.Fail(403, "Доступ к чужому обращению запрещён");
        }

        return ServiceResult<TicketResponse>.Ok(MapToResponse(ticket));
    }

    public async Task<ServiceResult<TicketResponse>> CreateAsync(CreateTicketRequest request, int createdByUserId, CancellationToken cancellationToken = default)
    {
        // Серверная проверка полей — та же, что на клиенте в ЛР5
        var title = request.Title.Trim();
        var description = request.Description.Trim();

        if (title.Length < 5 || title.Length > 80)
        {
            return ServiceResult<TicketResponse>.Fail(400, "Тема: от 5 до 80 символов");
        }

        if (description.Length < 10 || description.Length > 500)
        {
            return ServiceResult<TicketResponse>.Fail(400, "Описание: от 10 до 500 символов");
        }

        var room = await _db.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
        if (room is null)
        {
            return ServiceResult<TicketResponse>.Fail(400, "Комната не найдена в справочнике");
        }

        var ticket = new Ticket
        {
            Number = await NextNumberAsync(cancellationToken),
            Title = title,
            Description = description,
            // Статус и номер задаёт сервер; клиент их не присылает
            Status = TicketStatuses.New,
            RoomId = request.RoomId,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTimeOffset.UtcNow,
            AssigneeUserId = null
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponse>.Ok(MapToResponse(ticket));
    }

    public async Task<ServiceResult<TicketResponse>> AssignAsync(int id, AssignTicketRequest request, int userId, string role, CancellationToken cancellationToken = default)
    {
        if (!TicketRoles.IsCommandant(role))
        {
            return ServiceResult<TicketResponse>.Fail(403, "Назначать исполнителя может только комендант");
        }

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<TicketResponse>.Fail(404, "Обращение не найдено");
        }

        // ТЗ ЛР1, требование 3: назначить исполнителя можно только в статусе New
        if (ticket.Status != TicketStatuses.New)
        {
            return ServiceResult<TicketResponse>.Fail(409, "Назначить исполнителя можно только для обращения в статусе New");
        }

        var assignee = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.AssigneeUserId, cancellationToken);
        if (assignee is null)
        {
            return ServiceResult<TicketResponse>.Fail(400, "Пользователь-исполнитель не найден");
        }

        if (!TicketRoles.IsExecutor(assignee.Role))
        {
            return ServiceResult<TicketResponse>.Fail(400, "Исполнитель должен быть сантехником или электриком");
        }

        ticket.AssigneeUserId = assignee.Id;
        await _db.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponse>.Ok(MapToResponse(ticket));
    }

    public async Task<ServiceResult<TicketResponse>> ChangeStatusAsync(int id, UpdateTicketStatusRequest request, int userId, string role, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (ticket is null)
        {
            return ServiceResult<TicketResponse>.Fail(404, "Обращение не найдено");
        }

        var newStatus = request.Status.Trim();
        if (!TicketStatuses.All.Contains(newStatus))
        {
            return ServiceResult<TicketResponse>.Fail(400, $"Статус должен быть одним из: {string.Join(", ", TicketStatuses.All)}");
        }

        if (TicketRoles.IsCommandant(role))
        {
            // ТЗ ЛР1: комендант не меняет статус работы, он может только отменить обращение
            if (newStatus != TicketStatuses.Cancelled)
            {
                return ServiceResult<TicketResponse>.Fail(403, "Комендант может только отменить обращение (Cancelled), статус работы меняет исполнитель");
            }

            if (ticket.Status != TicketStatuses.New)
            {
                return ServiceResult<TicketResponse>.Fail(409, "Отменить можно только обращение в статусе New");
            }
        }
        else
        {
            // Правило процесса из ЛР8: нельзя перевести в работу без исполнителя — 409
            if (newStatus == TicketStatuses.InProgress && ticket.AssigneeUserId is null)
            {
                return ServiceResult<TicketResponse>.Fail(409, "Нельзя перевести в работу без назначенного исполнителя");
            }

            // ЛР11: исполнитель меняет статус только обращения, назначенного на него
            if (ticket.AssigneeUserId != userId)
            {
                return ServiceResult<TicketResponse>.Fail(403, "Исполнитель может менять статус только обращения, назначенного на него");
            }

            var allowed = (ticket.Status, newStatus) switch
            {
                (TicketStatuses.New, TicketStatuses.InProgress) => true,
                (TicketStatuses.InProgress, TicketStatuses.Closed) => true,
                _ => false
            };

            if (!allowed)
            {
                return ServiceResult<TicketResponse>.Fail(409, $"Переход {ticket.Status} → {newStatus} не разрешён");
            }
        }

        ticket.Status = newStatus;
        await _db.SaveChangesAsync(cancellationToken);

        return ServiceResult<TicketResponse>.Ok(MapToResponse(ticket));
    }

    // Человеческий номер ОБР-2026-001: сервер считает последовательность
    // внутри текущего года. Гонка при одновременном создании — известное
    // учебное ограничение (см. docs/known-defects.md).
    private async Task<string> NextNumberAsync(CancellationToken cancellationToken)
    {
        var year = DateTimeOffset.UtcNow.Year;
        var prefix = $"ОБР-{year}-";

        var numbers = await _db.Tickets
            .Where(t => t.Number.StartsWith(prefix))
            .Select(t => t.Number)
            .ToListAsync(cancellationToken);

        var max = 0;
        foreach (var number in numbers)
        {
            if (int.TryParse(number[prefix.Length..], out var sequence) && sequence > max)
            {
                max = sequence;
            }
        }

        return $"{prefix}{(max + 1):D3}";
    }

    private static TicketResponse MapToResponse(Ticket ticket) => new()
    {
        Id = ticket.Id,
        Number = ticket.Number,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        RoomId = ticket.RoomId,
        CreatedAt = ticket.CreatedAt,
        CreatedByUserId = ticket.CreatedByUserId,
        AssigneeUserId = ticket.AssigneeUserId
    };
}
