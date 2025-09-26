using OnlineTutoringSystem.Domain.Course;
using OnlineTutoringSystem.Domain.Course.Repositories;
using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class CourseManagementService : DomainServiceBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly IPersonRepository _personRepository;

    public CourseManagementService(IClock clock, ICourseRepository courseRepository, IPersonRepository personRepository) : base(clock)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<CourseAggregate> CreateCourseAsync(string title, string description, Subject subject, Guid tutorId, Money pricePerHour, Duration duration, CourseLevel level)
    {
        // Verify tutor exists and has tutor role
        var tutor = await _personRepository.GetByIdAsync(tutorId);
        if (tutor == null)
            throw new DomainException("Tutor not found.");

        var tutorRole = tutor.GetRole<TutorRole>();
        if (tutorRole == null)
            throw new DomainException("Person is not registered as a tutor.");

        if (!tutorRole.IsVerified)
            throw new DomainException("Only verified tutors can create courses.");

        if (!tutorRole.CanTeachSubject(subject))
            throw new DomainException("Tutor cannot teach this subject.");

        var course = new CourseAggregate(title, description, subject, tutorId, pricePerHour, duration, level);
        await _courseRepository.SaveAsync(course);
        return course;
    }

    public async Task UpdateCourseAsync(Guid courseId, string? title = null, string? description = null, Money? pricePerHour = null, Duration? duration = null, CourseLevel? level = null)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new DomainException("Course not found.");

        if (title != null)
            course.UpdateTitle(title);

        if (description != null)
            course.UpdateDescription(description);

        if (pricePerHour != null)
            course.UpdatePrice(pricePerHour);

        if (duration != null)
            course.UpdateDuration(duration);

        if (level != null)
            course.UpdateLevel(level.Value);

        await _courseRepository.SaveAsync(course);
    }

    public async Task DeactivateCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            throw new DomainException("Course not found.");

        course.Deactivate();
        await _courseRepository.SaveAsync(course);
    }

    public async Task<List<CourseAggregate>> GetCoursesBySubjectAsync(string subjectName)
    {
        return (await _courseRepository.GetBySubjectAsync(subjectName)).ToList();
    }

    public async Task<List<CourseAggregate>> GetCoursesByTutorAsync(Guid tutorId)
    {
        return (await _courseRepository.GetByTutorIdAsync(tutorId)).ToList();
    }

    public async Task<List<CourseAggregate>> GetActiveCoursesAsync()
    {
        return (await _courseRepository.GetActiveCoursesAsync()).ToList();
    }

    public async Task<List<CourseAggregate>> SearchCoursesAsync(string? subjectName = null, CourseLevel? level = null, decimal? maxPrice = null)
    {
        var courses = await _courseRepository.GetActiveCoursesAsync();

        if (!string.IsNullOrWhiteSpace(subjectName))
            courses = courses.Where(c => c.Subject.Name.Contains(subjectName, StringComparison.OrdinalIgnoreCase));

        if (level.HasValue)
            courses = courses.Where(c => c.Level == level.Value);

        if (maxPrice.HasValue)
            courses = courses.Where(c => c.PricePerHour.Amount <= maxPrice.Value);

        return courses.ToList();
    }
}
