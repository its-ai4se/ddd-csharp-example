using OnlineTutoringSystem.Domain.Course;
using OnlineTutoringSystem.Domain.Payment;
using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Session;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class DomainModelDemoTests
{
    [Fact]
    public void CreatePersonWithTutorAndStudentRoles()
    {
        // Arrange
        var name = new PersonName("John", "Doe");
        var email = new EmailAddress("john.doe@example.com");
        var phone = new PhoneNumber("+1234567890");
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var person = new PersonAggregate(name, email, dateOfBirth, phone);

        // Assert
        Assert.Equal("John Doe", person.Name.FullName);
        Assert.Equal("john.doe@example.com", person.EmailAddress.Value);
        Assert.True(person.GetAge() >= 34); // Age should be at least 34 (born in 1990)
    }

    [Fact]
    public void CreateTutorRole()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var subjects = new List<Subject>
        {
            new Subject("Mathematics", "Algebra and Calculus"),
            new Subject("Physics", "Mechanics and Thermodynamics")
        };
        var hourlyRate = new Money(50.00m, "USD");
        var bio = "Experienced math and physics tutor with 10 years of experience.";

        // Act
        var tutorRole = new TutorRole(personId, subjects, hourlyRate, bio);

        // Assert
        Assert.Equal(personId, tutorRole.PersonId);
        Assert.Equal(2, tutorRole.Subjects.Count);
        Assert.Equal(50.00m, tutorRole.HourlyRate.Amount);
        Assert.False(tutorRole.IsVerified);
    }

    [Fact]
    public void CreateStudentRole()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var interestedSubjects = new List<Subject>
        {
            new Subject("Mathematics"),
            new Subject("Computer Science")
        };
        var learningGoals = "Prepare for university entrance exams";
        var preferredStyle = "Interactive learning";

        // Act
        var studentRole = new StudentRole(personId, interestedSubjects, learningGoals, preferredStyle);

        // Assert
        Assert.Equal(personId, studentRole.PersonId);
        Assert.Equal(2, studentRole.InterestedSubjects.Count);
        Assert.Equal("Prepare for university entrance exams", studentRole.LearningGoals);
    }

    [Fact]
    public void CreateCourse()
    {
        // Arrange
        var title = "Advanced Calculus";
        var description = "Comprehensive course covering differential and integral calculus";
        var subject = new Subject("Mathematics", "Advanced calculus topics");
        var tutorId = Guid.NewGuid();
        var pricePerHour = new Money(75.00m, "USD");
        var duration = Duration.FromHours(1);
        var level = CourseLevel.Advanced;

        // Act
        var course = new CourseAggregate(title, description, subject, tutorId, pricePerHour, duration, level);

        // Assert
        Assert.Equal("Advanced Calculus", course.Title);
        Assert.Equal(CourseStatus.Active, course.Status);
        Assert.Equal(CourseLevel.Advanced, course.Level);
        Assert.Equal(75.00m, course.PricePerHour.Amount);
    }

    [Fact]
    public void CreateSession()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var tutorId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var scheduledStartTime = DateTime.UtcNow.AddDays(1);
        var duration = Duration.FromHours(1);
        var price = new Money(75.00m, "USD");

        // Act
        var session = new SessionAggregate(courseId, tutorId, studentId, scheduledStartTime, duration, price);

        // Assert
        Assert.Equal(SessionStatus.Scheduled, session.Status);
        Assert.Equal(courseId, session.CourseId);
        Assert.Equal(tutorId, session.TutorId);
        Assert.Equal(studentId, session.StudentId);
        Assert.Equal(75.00m, session.Price.Amount);
    }

    [Fact]
    public void CreatePayment()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var tutorId = Guid.NewGuid();
        var amount = new Money(75.00m, "USD");
        var method = PaymentMethod.CreditCard;

        // Act
        var payment = new PaymentAggregate(sessionId, studentId, tutorId, amount, method);

        // Assert
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(PaymentMethod.CreditCard, payment.Method);
        Assert.Equal(75.00m, payment.Amount.Amount);
    }

    [Fact]
    public void SessionLifecycle()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var tutorId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var scheduledStartTime = DateTime.UtcNow.AddDays(1);
        var duration = Duration.FromHours(1);
        var price = new Money(75.00m, "USD");

        var session = new SessionAggregate(courseId, tutorId, studentId, scheduledStartTime, duration, price);

        // Act & Assert - Start session
        session.Start("https://meet.example.com/session123");
        Assert.Equal(SessionStatus.InProgress, session.Status);
        Assert.NotNull(session.ActualStartTime);
        Assert.Equal("https://meet.example.com/session123", session.MeetingLink);

        // Complete session
        session.Complete("Great session! Student understood the concepts well.");
        Assert.Equal(SessionStatus.Completed, session.Status);
        Assert.NotNull(session.ActualEndTime);
        Assert.Equal("Great session! Student understood the concepts well.", session.Notes);
    }

    [Fact]
    public void PaymentLifecycle()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var tutorId = Guid.NewGuid();
        var amount = new Money(75.00m, "USD");
        var method = PaymentMethod.CreditCard;

        var payment = new PaymentAggregate(sessionId, studentId, tutorId, amount, method);

        // Act & Assert - Process payment
        payment.Process("txn_123456789");
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.Equal("txn_123456789", payment.TransactionId);
        Assert.NotNull(payment.ProcessedAt);

        // Refund payment
        payment.Refund("Student requested refund due to technical issues");
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.Equal("Student requested refund due to technical issues", payment.FailureReason);
    }

    [Fact]
    public void ValueObjectEquality()
    {
        // Arrange
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Doe");
        var name3 = new PersonName("Jane", "Doe");

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.NotEqual(name1, name3);
        Assert.True(name1 == name2);
        Assert.False(name1 == name3);
    }

    [Fact]
    public void MoneyOperations()
    {
        // Arrange
        var money1 = new Money(50.00m, "USD");
        var money2 = new Money(25.00m, "USD");

        // Act & Assert
        var sum = money1 + money2;
        var difference = money1 - money2;
        var multiplied = money1 * 2;

        Assert.Equal(75.00m, sum.Amount);
        Assert.Equal(25.00m, difference.Amount);
        Assert.Equal(100.00m, multiplied.Amount);
    }

    [Fact]
    public void DurationOperations()
    {
        // Arrange
        var duration1 = Duration.FromHours(1);
        var duration2 = Duration.FromMinutes(30);

        // Act & Assert
        Assert.Equal(60, duration1.Minutes);
        Assert.Equal(30, duration2.Minutes);
        Assert.Equal(TimeSpan.FromHours(1), duration1.ToTimeSpan());
    }
}
