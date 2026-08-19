using CodeReviewer.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CodeReviewer.Mvc.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Services.N8nService _n8nService;
        public StudentController(IHttpClientFactory httpClientFactory, Services.N8nService n8nService)
        {
            _httpClientFactory = httpClientFactory;
            _n8nService = n8nService;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Assignments(int classroomId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int studentId = int.TryParse(userIdString, out var id) ? id : 0;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Assignments/classroom/{classroomId}");
            var assignments = new List<AssignmentViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                assignments = JsonSerializer.Deserialize<List<AssignmentViewModel>>(jsonString, options) ?? new List<AssignmentViewModel>();
                foreach (var assignment in assignments)
                {
                    var lockResponse = await client.GetAsync($"api/Submissions/check/{studentId}/{assignment.Id}");
                    if (lockResponse.IsSuccessStatusCode)
                    {
                        var lockJson = await lockResponse.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(lockJson);
                        assignment.HasSubmitted = doc.RootElement.GetProperty("hasSubmitted").GetBoolean();
                        if (assignment.HasSubmitted)
                        {
                            assignment.SubmissionId = doc.RootElement.GetProperty("submissionId").GetInt32();
                            assignment.HasGrade = doc.RootElement.GetProperty("hasGrade").GetBoolean();
                        }
                    }
                }
            }
            ViewBag.ClassroomId = classroomId;
            return View(assignments);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int assignmentId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int studentId = int.TryParse(userIdString, out var id) ? id : 0;
            var model = new SubmissionViewModel
            {
                StudentId = studentId,
                AssignmentId = assignmentId
            };
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Assignments/{assignmentId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var assignment = JsonSerializer.Deserialize<AssignmentViewModel>(jsonString, options);
                if (assignment != null)
                {
                    model.AssignmentTitle = assignment.Title;
                    model.AssignmentDescription = assignment.Description;
                    model.AllowedLanguage = string.IsNullOrWhiteSpace(assignment.AllowedLanguage) ? "Any" : assignment.AllowedLanguage;
                    model.ClassroomId = assignment.ClassroomId;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCode(SubmissionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CodeText))
            {
                ModelState.AddModelError("", "Code content cannot be empty.");
                return View("Index", model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonPayload = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Submissions", jsonPayload);
            if (response.IsSuccessStatusCode)
            {
                // Fetch the exact assignment details from the API
                var assignResponse = await client.GetAsync($"api/Assignments/{model.AssignmentId}");
                if (assignResponse.IsSuccessStatusCode)
                {
                    var assignJson = await assignResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var assignment = JsonSerializer.Deserialize<AssignmentViewModel>(assignJson, options);
                    if (assignment != null)
                    {
                        model.AssignmentTitle = assignment.Title;
                        model.AssignmentDescription = assignment.Description;
                    }
                }

                var aiPayload = new
                {
                    StudentId = model.StudentId,
                    AssignmentId = model.AssignmentId,
                    Language = model.Language,
                    Code = model.CodeText,
                    AssignmentTitle = model.AssignmentTitle,
                    AssignmentDescription = model.AssignmentDescription
                };

                // Fire the Webhook to n8n
                _ = _n8nService.SendSubmissionToAIAsync(aiPayload);

                TempData["SweetSuccess"] = "Your code has been submitted successfully!";
                return RedirectToAction("Assignments", new { classroomId = model.ClassroomId });
            }
            ModelState.AddModelError("", "Failed to connect to the backend server. Ensure Web API is running.");
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewSubmission(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Submissions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var submission = System.Text.Json.JsonSerializer.Deserialize<Models.SubmissionViewModel>(jsonString, options);
                if (submission != null)
                {
                    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int studentId = int.TryParse(userIdString, out var uid) ? uid : 0;
                    if (submission.StudentId == studentId)
                    {
                        return View("~/Views/Teacher/ViewSubmission.cshtml", submission);
                    }
                }
            }
            TempData["ErrorMessage"] = "Could not load feedback.";
            return RedirectToAction("Dashboard");
        }
    }
}