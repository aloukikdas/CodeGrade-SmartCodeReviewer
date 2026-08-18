using CodeReviewer.Api.Data;
using CodeReviewer.Api.DTOs;
using CodeReviewer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeReviewer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Assignments/classroom/{classroomId}
        [HttpGet("classroom/{classroomId}")]
        public async Task<IActionResult> GetAssignmentsByClassroom(int classroomId)
        {
            var assignments = await _context.Assignments
                .Where(a => a.ClassroomId == classroomId)
                .ToListAsync();

            return Ok(assignments);
        }

        // POST: api/Assignments
        [HttpPost]
        public async Task<IActionResult> CreateAssignment(CreateAssignmentDto request)
        {
            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                ClassroomId = request.ClassroomId,
                AllowedLanguage = request.AllowedLanguage
            };
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok(assignment);
        }

        // GET: api/Assignments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return Ok(assignment);
        }

        // PUT: api/Assignments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, CreateAssignmentDto request)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            assignment.Title = request.Title;
            assignment.Description = request.Description;
            assignment.AllowedLanguage = request.AllowedLanguage;
            await _context.SaveChangesAsync();
            return Ok(assignment);
        }

        // DELETE: api/Assignments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}