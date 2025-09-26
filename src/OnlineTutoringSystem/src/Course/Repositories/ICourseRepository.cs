using OnlineTutoringSystem.Domain.Course;

namespace OnlineTutoringSystem.Domain.Course.Repositories;

public interface ICourseRepository
{
    Task<CourseAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<CourseAggregate>> GetByTutorIdAsync(Guid tutorId);
    Task<IEnumerable<CourseAggregate>> GetBySubjectAsync(string subjectName);
    Task<IEnumerable<CourseAggregate>> GetByLevelAsync(CourseLevel level);
    Task<IEnumerable<CourseAggregate>> GetActiveCoursesAsync();
    Task<IEnumerable<CourseAggregate>> GetAllAsync();
    Task SaveAsync(CourseAggregate course);
    Task DeleteAsync(Guid id);
}
