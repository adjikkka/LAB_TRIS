namespace backend.Models;

// Справочник комнат (ЛР2): в обращении хранится только roomId.
public sealed class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
