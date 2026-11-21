using backend.Auth;
using backend.Dtos;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

// REST главного ресурса (ЛР8). Контроллер тонкий: принял HTTP,
// вызвал сервис, вернул код. DELETE в курсе не используется.
[ApiController]
[Route("api/tickets")]
[Authorize]
public sealed class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets)
    {
        _tickets = tickets;
    }

    // Создание — только комендант (матрица ролей, docs/role-matrix.md).
    // Исполнителю — 403, гостю — 401.
    [HttpPost]
    [Authorize(Roles = TicketRoles.Commandant)]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var result = await _tickets.CreateAsync(request, User.GetUserId(), cancellationToken);
        if (!result.Success)
        {
            return Problem(statusCode: result.StatusCode!.Value, title: result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    // Пустой список — 200 и [], не 404 (ЛР8).
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _tickets.GetTicketsAsync(User.GetUserId(), User.GetUserRole(), cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _tickets.GetTicketAsync(id, User.GetUserId(), User.GetUserRole(), cancellationToken);
        if (!result.Success)
        {
            return Problem(statusCode: result.StatusCode!.Value, title: result.Error);
        }

        return Ok(result.Value);
    }

    // Назначение исполнителя — отдельное действие и отдельный PATCH.
    [HttpPatch("{id:int}/assignee")]
    [Authorize(Roles = TicketRoles.Commandant)]
    public async Task<ActionResult<TicketResponse>> Assign(int id, AssignTicketRequest request, CancellationToken cancellationToken)
    {
        var result = await _tickets.AssignAsync(id, request, User.GetUserId(), User.GetUserRole(), cancellationToken);
        if (!result.Success)
        {
            return Problem(statusCode: result.StatusCode!.Value, title: result.Error);
        }

        return Ok(result.Value);
    }

    // Смена статуса — отдельное действие и отдельный PATCH.
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TicketResponse>> ChangeStatus(int id, UpdateTicketStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _tickets.ChangeStatusAsync(id, request, User.GetUserId(), User.GetUserRole(), cancellationToken);
        if (!result.Success)
        {
            return Problem(statusCode: result.StatusCode!.Value, title: result.Error);
        }

        return Ok(result.Value);
    }
}
