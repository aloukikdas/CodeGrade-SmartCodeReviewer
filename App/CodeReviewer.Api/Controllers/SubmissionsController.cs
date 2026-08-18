using CodeReviewer.Api.Data;
using CodeReviewer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeReviewer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SubmissionsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Submissions
        [HttpPost]
        public async Task<IActionResult> CreateSubmission(Submission submission)
        {
            var existingSubmission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.StudentId == submission.StudentId && s.AssignmentId == submission.AssignmentId);
            if (existingSubmission != null)
            {
                return BadRequest("You have already submitted this assignment. Multiple attempts are not allowed.");
            }
            submission.SubmissionDate = DateTime.UtcNow;
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            return Ok(submission);
        }

        // GET: api/Submissions/check/{studentId}/{assignmentId} 
        [HttpGet("check/{studentId}/{assignmentId}")]
        public async Task<IActionResult> CheckSubmissionStatus(int studentId, int assignmentId)
        {
            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.AssignmentId == assignmentId);
            if (submission != null)
            {
                return Ok(new
                {
                    HasSubmitted = true,
                    SubmissionId = submission.Id,
                    HasGrade = submission.Grade.HasValue
                });
            }
            return Ok(new { HasSubmitted = false });
        }

        // GET: api/Submissions
        [HttpGet]
        public async Task<IActionResult> GetSubmissions()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .ToListAsync();
            return Ok(submissions);
        }

        // DELETE: api/Submissions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null) return NotFound();
            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Submissions/classroom/{classroomId}
        [HttpGet("classroom/{classroomId}")]
        public async Task<IActionResult> GetSubmissionsByClassroom(int classroomId)
        {
            var assignments = await _context.Assignments
                .Where(a => a.ClassroomId == classroomId)
                .ToListAsync();
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.ClassroomId == classroomId)
                .Select(e => e.Student)
                .ToListAsync();
            var submissions = await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.Assignment!.ClassroomId == classroomId)
                .ToListAsync();

            var dashboardList = new List<object>();
            foreach (var assignment in assignments)
            {
                foreach (var student in students)
                {
                    var sub = submissions.FirstOrDefault(s => s.AssignmentId == assignment.Id && s.StudentId == student!.Id);
                    if (sub != null)
                    {
                        dashboardList.Add(new
                        {
                            id = sub.Id.ToString(),
                            student = new { name = sub.Student!.Name, email = sub.Student.Email },
                            assignment = new { title = sub.Assignment!.Title },
                            language = sub.Language,
                            submissionDate = sub.SubmissionDate.ToString("o"),
                            status = "submitted",
                            grade = sub.Grade
                        });
                    }
                    else
                    {
                        dashboardList.Add(new
                        {
                            id = (string?)null,
                            student = new { name = student!.Name, email = student.Email },
                            assignment = new { title = assignment.Title },
                            language = "N/A",
                            submissionDate = (string?)null,
                            status = "not_submitted",
                            grade = (int?)null
                        });
                    }
                }
            }
            return Ok(dashboardList);
        }

        [HttpPost("grade")]
        public async Task<IActionResult> ReceiveAiGrade([FromBody] GradingPayload payload)
        {
            if (payload == null)
            {
                return BadRequest("Invalid payload.");
            }

            // 1. Find the exact submission matching the Student and Assignment
            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.StudentId == payload.StudentId && s.AssignmentId == payload.AssignmentId);

            if (submission == null)
            {
                return NotFound("Could not find a matching submission to grade.");
            }

            // 2. Update the database record with the AI's results
            submission.Grade = payload.Grade;
            submission.AiFeedback = payload.AiFeedback;

            // 3. Save the changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "Grade successfully saved to database!" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissionById(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound("Submission not found.");
            }

            var result = new
            {
                studentId = submission.StudentId,
                assignmentId = submission.AssignmentId,
                classroomId = submission.Assignment!.ClassroomId,
                codeText = submission.CodeText,
                assignmentTitle = submission.Assignment.Title,
                assignmentDescription = submission.Assignment.Description,
                language = submission.Language,
                grade = submission.Grade,
                aiFeedback = submission.AiFeedback
            };

            return Ok(result);
        }
    }
}