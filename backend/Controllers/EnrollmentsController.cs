using backend.Contracts.Enrollments;
using backend.Data;
using backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace backend.Controllers;

[ApiController]
[Route("enrollments")]
public sealed class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public EnrollmentsController(AppDbContext db)
    {
        _db = db;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<EnrollmentDto>>> MyEnrollments(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var enrollments = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.DeletedAt == null)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EnrollmentDto(
                e.Id,
                e.CourseId,
                e.Course.Title,
                e.Status,
                e.Progress,
                e.EnrolledOn
            ))
            .ToListAsync(cancellationToken);

        return Ok(enrollments);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> Enroll([FromBody] EnrollRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var courseExists = await _db.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.CourseId && c.DeletedAt == null && c.IsPublished, cancellationToken);

        if (!courseExists)
        {
            return NotFound("Course not found.");
        }

        var alreadyEnrolled = await _db.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.UserId == userId && e.CourseId == request.CourseId && e.DeletedAt == null, cancellationToken);

        if (alreadyEnrolled)
        {
            return Conflict("Already enrolled.");
        }

        var now = DateTime.UtcNow;
        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Active,
            Progress = 0m,
            EnrolledOn = DateOnly.FromDateTime(now),
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == enrollment.Id)
            .Select(e => new EnrollmentDto(
                e.Id,
                e.CourseId,
                e.Course.Title,
                e.Status,
                e.Progress,
                e.EnrolledOn
            ))
            .SingleAsync(cancellationToken);

        return Ok(dto);
    }

    private long GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(sub) || !long.TryParse(sub, out var userId))
        {
            throw new InvalidOperationException("Missing or invalid JWT sub claim.");
        }

        return userId;
    }
}
