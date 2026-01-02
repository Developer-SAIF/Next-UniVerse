using backend.Contracts.Courses;
using backend.Data;
using backend.Entities;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Authorize(Policy = "InstructorOnly")]
[Route("instructor")]
public sealed class InstructorContentController : ControllerBase
{
    private readonly AppDbContext _db;

    public InstructorContentController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("courses/{courseId:long}/modules")]
    public async Task<ActionResult<IReadOnlyList<ModuleDto>>> ListModules([FromRoute] long courseId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var owns = await _db.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Id == courseId && c.DeletedAt == null && c.InstructorId == userId.Value, cancellationToken);

        if (!owns)
        {
            // Avoid leaking existence of other instructors' courses.
            return NotFound();
        }

        var modules = await _db.Modules
            .AsNoTracking()
            .Where(m => m.CourseId == courseId && m.DeletedAt == null)
            .OrderBy(m => m.Position)
            .ThenBy(m => m.Id)
            .Select(m => new ModuleDto(
                m.Id,
                m.Title,
                m.Position,
                m.Description,
                m.Materials
                    .Where(x => x.DeletedAt == null)
                    .OrderBy(x => x.Position)
                    .ThenBy(x => x.Id)
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
            .ToListAsync(cancellationToken);

        return Ok(modules);
    }

    [HttpPost("courses/{courseId:long}/modules")]
    public async Task<ActionResult<ModuleDto>> CreateModule([FromRoute] long courseId, [FromBody] CreateModuleRequest request, CancellationToken cancellationToken)
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

        var course = await _db.Courses
            .SingleOrDefaultAsync(c => c.Id == courseId && c.DeletedAt == null, cancellationToken);

        if (course is null || course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        var module = new Module
        {
            CourseId = courseId,
            Title = request.Title.Trim(),
            Position = request.Position,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        _db.Modules.Add(module);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ModuleDto(
            module.Id,
            module.Title,
            module.Position,
            module.Description,
            new List<MaterialDto>()
        ));
    }

    [HttpPut("modules/{moduleId:long}")]
    public async Task<ActionResult> UpdateModule([FromRoute] long moduleId, [FromBody] UpdateModuleRequest request, CancellationToken cancellationToken)
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

        var module = await _db.Modules
            .Include(m => m.Course)
            .SingleOrDefaultAsync(m => m.Id == moduleId && m.DeletedAt == null, cancellationToken);

        if (module is null)
        {
            return NotFound();
        }

        if (module.Course.DeletedAt != null || module.Course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        module.Title = request.Title.Trim();
        module.Position = request.Position;
        module.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        module.UpdatedAt = DateTime.UtcNow;
        module.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("modules/{moduleId:long}")]
    public async Task<ActionResult> DeleteModule([FromRoute] long moduleId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var module = await _db.Modules
            .Include(m => m.Course)
            .SingleOrDefaultAsync(m => m.Id == moduleId && m.DeletedAt == null, cancellationToken);

        if (module is null)
        {
            return NotFound();
        }

        if (module.Course.DeletedAt != null || module.Course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        module.DeletedAt = now;
        module.UpdatedAt = now;
        module.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("modules/{moduleId:long}/materials")]
    public async Task<ActionResult<MaterialDto>> CreateMaterial([FromRoute] long moduleId, [FromBody] CreateMaterialRequest request, CancellationToken cancellationToken)
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

        var module = await _db.Modules
            .Include(m => m.Course)
            .SingleOrDefaultAsync(m => m.Id == moduleId && m.DeletedAt == null, cancellationToken);

        if (module is null)
        {
            return NotFound();
        }

        if (module.Course.DeletedAt != null || module.Course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        var material = new Material
        {
            ModuleId = moduleId,
            Kind = request.Kind,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            StorageUrl = string.IsNullOrWhiteSpace(request.StorageUrl) ? null : request.StorageUrl.Trim(),
            MimeType = string.IsNullOrWhiteSpace(request.MimeType) ? null : request.MimeType.Trim(),
            SizeBytes = request.SizeBytes,
            RequiresAuth = request.RequiresAuth,
            Position = request.Position,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        _db.Materials.Add(material);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new MaterialDto(
            material.Id,
            material.Kind,
            material.Title,
            material.Description,
            material.StorageUrl,
            material.MimeType,
            material.SizeBytes,
            material.RequiresAuth,
            material.Position
        ));
    }

    [HttpPut("materials/{materialId:long}")]
    public async Task<ActionResult> UpdateMaterial([FromRoute] long materialId, [FromBody] UpdateMaterialRequest request, CancellationToken cancellationToken)
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

        var material = await _db.Materials
            .Include(m => m.Module)
            .ThenInclude(m => m.Course)
            .SingleOrDefaultAsync(m => m.Id == materialId && m.DeletedAt == null, cancellationToken);

        if (material is null)
        {
            return NotFound();
        }

        if (material.Module.DeletedAt != null || material.Module.Course.DeletedAt != null || material.Module.Course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        material.Kind = request.Kind;
        material.Title = request.Title.Trim();
        material.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        material.StorageUrl = string.IsNullOrWhiteSpace(request.StorageUrl) ? null : request.StorageUrl.Trim();
        material.MimeType = string.IsNullOrWhiteSpace(request.MimeType) ? null : request.MimeType.Trim();
        material.SizeBytes = request.SizeBytes;
        material.RequiresAuth = request.RequiresAuth;
        material.Position = request.Position;
        material.UpdatedAt = DateTime.UtcNow;
        material.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("materials/{materialId:long}")]
    public async Task<ActionResult> DeleteMaterial([FromRoute] long materialId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var material = await _db.Materials
            .Include(m => m.Module)
            .ThenInclude(m => m.Course)
            .SingleOrDefaultAsync(m => m.Id == materialId && m.DeletedAt == null, cancellationToken);

        if (material is null)
        {
            return NotFound();
        }

        if (material.Module.DeletedAt != null || material.Module.Course.DeletedAt != null || material.Module.Course.InstructorId != userId.Value)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        material.DeletedAt = now;
        material.UpdatedAt = now;
        material.UpdatedBy = userId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
