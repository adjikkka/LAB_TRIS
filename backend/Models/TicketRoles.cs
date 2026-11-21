namespace backend.Models;

// Роли из таблицы соответствия ЛР1 (project-proposal.md).
public static class TicketRoles
{
    public const string Commandant = "Комендант";
    public const string Plumber = "Сантехник";
    public const string Electrician = "Электрик";

    public static bool IsExecutor(string role) => role == Plumber || role == Electrician;

    public static bool IsCommandant(string role) => role == Commandant;
}

// Статусы обращения — ровно четыре значения из ЛР1/ЛР2.
public static class TicketStatuses
{
    public const string New = "New";
    public const string InProgress = "InProgress";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = { New, InProgress, Closed, Cancelled };
}
