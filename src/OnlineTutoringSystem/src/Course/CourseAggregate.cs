using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Course;

public class CourseAggregate : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Subject Subject { get; private set; }
    public Guid TutorId { get; private set; }
    public Money PricePerHour { get; private set; }
    public Duration Duration { get; private set; }
    public CourseLevel Level { get; private set; }
    public CourseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public CourseAggregate(Guid id, string title, string description, Subject subject, Guid tutorId, Money pricePerHour, Duration duration, CourseLevel level) : base(id)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        TutorId = tutorId;
        PricePerHour = pricePerHour ?? throw new ArgumentNullException(nameof(pricePerHour));
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Level = level;
        Status = CourseStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public CourseAggregate(string title, string description, Subject subject, Guid tutorId, Money pricePerHour, Duration duration, CourseLevel level) : base()
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        TutorId = tutorId;
        PricePerHour = pricePerHour ?? throw new ArgumentNullException(nameof(pricePerHour));
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Level = level;
        Status = CourseStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new DomainException("Course title cannot be empty.");

        Title = newTitle.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription ?? "";
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(Money newPrice)
    {
        PricePerHour = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDuration(Duration newDuration)
    {
        Duration = newDuration ?? throw new ArgumentNullException(nameof(newDuration));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLevel(CourseLevel newLevel)
    {
        Level = newLevel;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = CourseStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = CourseStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = CourseStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString() => $"Course: {Title} (ID: {Id})";
}

public enum CourseLevel
{
    Beginner,
    Intermediate,
    Advanced
}

public enum CourseStatus
{
    Active,
    Inactive,
    Archived
}
