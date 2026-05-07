using Microsoft.EntityFrameworkCore;
using SmartReminder.Domain.Entities;

namespace SmartReminder.Infrastructure.Persistence;

public class SmartReminderDbContext : DbContext
{
    public SmartReminderDbContext(DbContextOptions<SmartReminderDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<ReminderTask> ReminderTasks => Set<ReminderTask>();
    public DbSet<TaskStep> TaskSteps => Set<TaskStep>();

    public DbSet<StudentParentLink> StudentParentLinks => Set<StudentParentLink>();
    public DbSet<StudentTeacherLink> StudentTeacherLinks => Set<StudentTeacherLink>();

    public DbSet<PomodoroSession> PomodoroSessions => Set<PomodoroSession>();

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public DbSet<VisualSchedule> VisualSchedules => Set<VisualSchedule>();
    public DbSet<VisualScheduleItem> VisualScheduleItems => Set<VisualScheduleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAppUser(modelBuilder);
        ConfigureReminderTask(modelBuilder);
        ConfigureTaskStep(modelBuilder);
        ConfigureStudentParentLink(modelBuilder);
        ConfigureStudentTeacherLink(modelBuilder);
        ConfigurePomodoroSession(modelBuilder);
        ConfigureConversation(modelBuilder);
        ConfigureConversationParticipant(modelBuilder);
        ConfigureChatMessage(modelBuilder);
        ConfigureVisualSchedule(modelBuilder);
        ConfigureVisualScheduleItem(modelBuilder);
    }

    private static void ConfigureAppUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(x => x.Role)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(x => x.IsActive)
                .IsRequired();
        });
    }

    private static void ConfigureReminderTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReminderTask>(entity =>
        {
            entity.ToTable("ReminderTasks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.Priority)
                .HasConversion<int>();

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasOne(x => x.OwnerUser)
                .WithMany(x => x.OwnedTasks)
                .HasForeignKey(x => x.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedByUser)
                .WithMany(x => x.CreatedTasks)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.SourceChatMessage)
                .WithOne()
                .HasForeignKey<ReminderTask>(x => x.SourceChatMessageId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureTaskStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskStep>(entity =>
        {
            entity.ToTable("TaskSteps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(300);

            entity.HasOne(x => x.ReminderTask)
                .WithMany(x => x.Steps)
                .HasForeignKey(x => x.ReminderTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureStudentParentLink(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentParentLink>(entity =>
        {
            entity.ToTable("StudentParentLinks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.StudentId, x.ParentId })
                .IsUnique();
        });
    }

    private static void ConfigureStudentTeacherLink(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentTeacherLink>(entity =>
        {
            entity.ToTable("StudentTeacherLinks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Teacher)
                .WithMany()
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.StudentId, x.TeacherId })
                .IsUnique();
        });
    }

    private static void ConfigurePomodoroSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PomodoroSession>(entity =>
        {
            entity.ToTable("PomodoroSessions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.Property(x => x.Notes)
                .HasMaxLength(2000);

            entity.HasOne(x => x.User)
                .WithMany(x => x.PomodoroSessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ReminderTask)
                .WithMany(x => x.PomodoroSessions)
                .HasForeignKey(x => x.ReminderTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureConversation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .HasConversion<int>();

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);
        });
    }

    private static void ConfigureConversationParticipant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.ToTable("ConversationParticipants");

            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Conversation)
                .WithMany(x => x.Participants)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                .WithMany(x => x.ConversationParticipants)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ConversationId, x.UserId })
                .IsUnique();
        });
    }

    private static void ConfigureChatMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessages");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.MessageText)
                .IsRequired()
                .HasMaxLength(5000);

            entity.HasOne(x => x.Conversation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.SenderUser)
                .WithMany()
                .HasForeignKey(x => x.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedTask)
                .WithOne()
                .HasForeignKey<ChatMessage>(x => x.CreatedTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureVisualSchedule(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VisualSchedule>(entity =>
        {
            entity.ToTable("VisualSchedules");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.StudentId, x.ScheduleDate });
        });
    }

    private static void ConfigureVisualScheduleItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VisualScheduleItem>(entity =>
        {
            entity.ToTable("VisualScheduleItems");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.IconName)
                .HasMaxLength(100);

            entity.Property(x => x.ColorCode)
                .HasMaxLength(20);

            entity.Property(x => x.ItemType)
                .HasConversion<int>();

            entity.HasOne(x => x.VisualSchedule)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.VisualScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.LinkedTask)
                .WithMany(x => x.VisualScheduleItems)
                .HasForeignKey(x => x.LinkedTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}