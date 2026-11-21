using backend.Dtos;

namespace backend.Services;

// Контроллер зависит от интерфейса, а не от класса:
// реализацию хранения можно заменить, не переписывая контроллер (ЛР7).
public interface ITicketService
{
    Task<ServiceResult<IReadOnlyList<TicketResponse>>> GetTicketsAsync(int userId, string role, CancellationToken cancellationToken = default);

    Task<ServiceResult<TicketResponse>> GetTicketAsync(int id, int userId, string role, CancellationToken cancellationToken = default);

    Task<ServiceResult<TicketResponse>> CreateAsync(CreateTicketRequest request, int createdByUserId, CancellationToken cancellationToken = default);

    Task<ServiceResult<TicketResponse>> AssignAsync(int id, AssignTicketRequest request, int userId, string role, CancellationToken cancellationToken = default);

    Task<ServiceResult<TicketResponse>> ChangeStatusAsync(int id, UpdateTicketStatusRequest request, int userId, string role, CancellationToken cancellationToken = default);
}
