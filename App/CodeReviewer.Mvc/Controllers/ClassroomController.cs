using CodeReviewer.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CodeReviewer.Mvc.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ClassroomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private int GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out var id) ? id : 0;
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> TeacherIndex()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Classrooms/teacher/{GetUserId()}");
            var classrooms = new List<ClassroomViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                classrooms = JsonSerializer.Deserialize<List<ClassroomViewModel>>(jsonString, options) ?? new List<ClassroomViewModel>();
            }
            ViewBag.CreateForm = new CreateClassroomViewModel();
            return View(classrooms);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateClassroomViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("TeacherIndex");

            var payload = new { Name = model.Name, TeacherId = GetUserId() };
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Classrooms", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Classroom created successfully!";
            }
            return RedirectToAction("TeacherIndex");
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> TeacherAssignments(int classroomId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Assignments/classroom/{classroomId}");
            var assignments = new List<AssignmentViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                assignments = JsonSerializer.Deserialize<List<AssignmentViewModel>>(jsonString, options) ?? new List<AssignmentViewModel>();
            }
            ViewBag.ClassroomId = classroomId;
            ViewBag.CreateForm = new CreateAssignmentViewModel { ClassroomId = classroomId };
            return View(assignments);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> CreateAssignment(CreateAssignmentViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("TeacherAssignments", new { classroomId = model.ClassroomId });
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Assignments", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Assignment added successfully!";
            }
            return RedirectToAction("TeacherAssignments", new { classroomId = model.ClassroomId });
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> EditAssignment(AssignmentViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var payload = new { Title = model.Title, Description = model.Description, ClassroomId = model.ClassroomId, AllowedLanguage = model.AllowedLanguage };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Assignments/{model.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Assignment updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update assignment.";
            }
            return RedirectToAction("TeacherAssignments", new { classroomId = model.ClassroomId });
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> DeleteAssignment(int id, int classroomId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Assignments/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Assignment deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete assignment.";
            }
            return RedirectToAction("TeacherAssignments", new { classroomId = classroomId });
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> StudentIndex()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Classrooms/student/{GetUserId()}");
            var classrooms = new List<ClassroomViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                classrooms = JsonSerializer.Deserialize<List<ClassroomViewModel>>(jsonString, options) ?? new List<ClassroomViewModel>();
            }

            ViewBag.JoinForm = new JoinClassroomViewModel();
            return View(classrooms);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Join(JoinClassroomViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("StudentIndex");

            var payload = new { JoinCode = model.JoinCode, StudentId = GetUserId() };
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Classrooms/enroll", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully joined the classroom!";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Join Code or you are already enrolled.";
            }
            return RedirectToAction("StudentIndex");
        }
    }
}