using CodeReviewer.Api.Data;
using CodeReviewer.Api.DTOs;
using CodeReviewer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeReviewer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClassroomsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateClassroom(CreateClassroomDto request)
        {
            var classroom = new Classroom
            {
                Name = request.Name,
                TeacherId = request.TeacherId,
                JoinCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()
            };
            _context.Classrooms.Add(classroom);
            await _context.SaveChangesAsync();
            return Ok(classroom);
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudent(EnrollDto request)
        {
            var classroom = await _context.Classrooms
                .FirstOrDefaultAsync(c => c.JoinCode == request.JoinCode.ToUpper());

            if (classroom == null)
            {
                return NotFound("Invalid Join Code. Classroom not found.");
            }
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == request.StudentId && e.ClassroomId == classroom.Id);

            if (alreadyEnrolled)
            {
                return BadRequest("You are already enrolled in this classroom.");
            }

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                ClassroomId = classroom.Id
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully enrolled!", ClassroomId = classroom.Id });
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherClassrooms(int teacherId)
        {
            var classrooms = await _context.Classrooms
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();

            return Ok(classrooms);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentClassrooms(int studentId)
        {
            var classrooms = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Classroom)
                .Select(e => e.Classroom)
                .ToListAsync();
            return Ok(classrooms);
        }

        [HttpGet("classroom/{classroomId}/students")]
        public async Task<IActionResult> GetEnrolledStudents(int classroomId)
        {
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.ClassroomId == classroomId)
                .Select(e => new {
                    name = e.Student!.Name,
                    email = e.Student.Email,
                    joinedDate = e.JoinedDate
                })
                .ToListAsync();
            return Ok(students);
        }
    }
}