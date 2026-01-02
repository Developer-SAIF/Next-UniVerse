using backend.Contracts.Courses;
using backend.Data;
using backend.Entities;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("instructor/courses")]
[Authorize(Policy = "InstructorOnly")]
public sealed class InstructorCoursesController : ControllerBase
{
    private readonly AppDbContext _db;

    public InstructorCoursesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InstructorCourseListItemDto>>> ListMine(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var courses = await _db.Courses
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.InstructorId == userId.Value)
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new InstructorCourseListItemDto(
                c.Id,
                c.Title,
                c.Description,
                c.Language,
                c.Level,
                c.IsPublished,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return Ok(courses);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDetailDto>> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        var now = DateTime.UtcNow;
        var course = new Course
        {
            InstructorId = userId.Value,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Language = string.IsNullOrWhiteSpace(request.Language) ? null : request.Language.Trim(),
            Level = request.Level,
            IsPublished = false,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync(cancellationToken);

        // Return the same shape as GET /courses/{id}
        var dto = await _db.Courses
            .AsNoTracking()
            .Where(c => c.Id == course.Id)
            .Select(c => new CourseDetailDto(
                c.Id,
                c.Title,
                c.Description,
                c.Language,
                c.Level,
                c.IsPublished,
                c.InstructorId,
                c.Instructor.Name,
                new List<ModuleDto>()
            ))
            .SingleAsync(cancellationToken);

        return CreatedAtAction(
            actionName: "GetById",
            controllerName: "Courses",
            routeValues: new { id = course.Id },
            value: dto);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult> Update([FromRoute] long id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        var course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        if (course is null)
        {
            return NotFound();
        }

        if (course.InstructorId != userId.Value)
        {
            return Forbid();
        }

        course.Title = request.Title.Trim();
        course.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        course.Language = string.IsNullOrWhiteSpace(request.Language) ? null : request.Language.Trim();
        course.Level = request.Level;
        course.UpdatedAt = DateTime.UtcNow;
        course.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:long}/publish")]
    public async Task<ActionResult> Publish([FromRoute] long id, CancellationToken cancellationToken)
    {
        return await SetPublished(id, isPublished: true, cancellationToken);
    }

    [HttpPost("{id:long}/unpublish")]
    public async Task<ActionResult> Unpublish([FromRoute] long id, CancellationToken cancellationToken)
    {
        return await SetPublished(id, isPublished: false, cancellationToken);
    }

    private async Task<ActionResult> SetPublished(long id, bool isPublished, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var course = await _db.Courses.SingleOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, cancellationToken);
        if (course is null)
        {
            return NotFound();
        }

        if (course.InstructorId != userId.Value)
        {
            return Forbid();
        }

        course.IsPublished = isPublished;
        course.UpdatedAt = DateTime.UtcNow;
        course.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
