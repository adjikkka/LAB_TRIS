namespace backend.Models;

// Пользователь системы (ЛР2 + ЛР11: добавлены вход и роль).
public sealed class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    // Роль из ЛР1: Комендант, Сантехник, Электрик
    public string Role { get; set; } = string.Empty;

    // Хэш пароля (ASP.NET Identity PasswordHasher), не исходный пароль
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();

    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
}
