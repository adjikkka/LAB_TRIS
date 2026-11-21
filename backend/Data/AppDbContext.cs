using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

// DbContext — «какие таблицы видит программа» (ЛР10).
// Имена таблиц и столбцов заданы явно: они совпадают со схемой из db/schema.sql (ЛР9).
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.Property(r => r.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(r => r.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(u => u.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(u => u.Login).HasColumnName("login").IsRequired().HasMaxLength(50);
            entity.Property(u => u.Role).HasColumnName("role").IsRequired().HasMaxLength(30);
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Login).IsUnique();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets", table =>
                table.HasCheckConstraint(
                    "tickets_status_check",
                    "status IN ('New', 'InProgress', 'Closed', 'Cancelled')"));

            entity.Property(t => t.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(t => t.Number).HasColumnName("number").IsRequired().HasMaxLength(20);
            entity.Property(t => t.Title).HasColumnName("title").IsRequired().HasMaxLength(80);
            entity.Property(t => t.Description).HasColumnName("description").IsRequired().HasMaxLength(500);
            entity.Property(t => t.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(t => t.RoomId).HasColumnName("room_id");
            entity.Property(t => t.CreatedAt).HasColumnName("created_at");
            entity.Property(t => t.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(t => t.AssigneeUserId).HasColumnName("assignee_user_id");

            entity.HasOne(t => t.Room)
                .WithMany(r => r.Tickets)
                .HasForeignKey(t => t.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Assignee)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssigneeUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индексы из ЛР9: по статусу и по внешнему ключу справочника
            entity.HasIndex(t => t.Number).IsUnique();
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.RoomId);
            entity.HasIndex(t => t.AssigneeUserId);
        });
    }
}
