using backend.Contracts.Courses;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("courses")]
public sealed class CoursesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CoursesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseListItemDto>>> List(CancellationToken cancellationToken)
    {
        var courses = await _db.Courses
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.IsPublished)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseListItemDto(
                c.Id,
                c.Title,
                c.Description,
                c.Language,
                c.Level,
                c.IsPublished,
                c.InstructorId,
                c.Instructor.Name
            ))
            .ToListAsync(cancellationToken);

        return Ok(courses);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CourseDetailDto>> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var course = await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == id && c.DeletedAt == null)
            .Select(c => new CourseDetailDto(
                c.Id,
                c.Title,
                c.Description,
                c.Language,
                c.Level,
                c.IsPublished,
                c.InstructorId,
                c.Instructor.Name,
                c.Modules
                    .Where(m => m.DeletedAt == null)
                    .OrderBy(m => m.Position)
                    .Select(m => new ModuleDto(
                        m.Id,
                        m.Title,
                        m.Position,
                        m.Description,
                        m.Materials
                            .Where(x => x.DeletedAt == null)
                            .OrderBy(x => x.Position)
                            .Select(x => new MaterialDto(
                                x.Id,
                                x.Kind,
                                x.Title,
                                x.Description,
                                x.StorageUrl,
                                x.MimeType,
                                x.SizeBytes,
                                x.RequiresAuth,
                                x.Position
                            ))
                            .ToList()
                    ))
                    .ToList()
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return NotFound();
        }

        return Ok(course);
    }
}
