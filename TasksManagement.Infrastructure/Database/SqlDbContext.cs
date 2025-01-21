using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Infrastructure.Database;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Flunt.Notifications.Notification>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(100);

            entity.HasIndex(u => u.Id).HasDatabaseName("IX_Users_Id");
            entity.HasIndex(entity => entity.Name).HasDatabaseName("IX_Users_Name");
            entity.HasIndex(entity => entity.Role).HasDatabaseName("IX_Users_Role");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(p => p.UserId)
                  .IsRequired();

            entity.HasMany(p => p.Tasks)
                  .WithOne()
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<User>()
                  .WithMany(u => u.Projects)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Projects_Users");

            entity.HasIndex(p => p.Id).HasDatabaseName("IX_Projects_Id");
            entity.HasIndex(p => p.Name).HasDatabaseName("IX_Projects_Name");
            entity.HasIndex(p => p.UserId).HasDatabaseName("IX_Projects_UserId");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(150);
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.DueDate).IsRequired();
            entity.Property(t => t.CompletionDate).IsRequired(false);
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.Priority).IsRequired();
            entity.Property(t => t.ProjectId).IsRequired();

            entity.HasMany(p => p.Comments)
                  .WithOne()
                  .HasForeignKey(c => c.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => t.Id).HasDatabaseName("IX_Tasks_Id");
            entity.HasIndex(t => t.ProjectId).HasDatabaseName("IX_Tasks_ProjectId");
            entity.HasIndex(t => t.DueDate).HasDatabaseName("IX_Tasks_DueDate");
            entity.HasIndex(t => t.CompletionDate).HasDatabaseName("IX_Tasks_CompletionDate");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).IsRequired().HasMaxLength(300);
            entity.Property(c => c.CreatedAt).IsRequired();

            entity.HasIndex(c => c.Id).HasDatabaseName("IX_Comments_Id");
            entity.HasIndex(c => c.TaskId).HasDatabaseName("IX_Comments_TaskId");
        });
    }
}