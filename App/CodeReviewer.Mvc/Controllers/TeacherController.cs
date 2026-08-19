using CodeReviewer.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CodeReviewer.Mvc.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeacherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ClassroomSubmissions(int classroomId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Submissions/classroom/{classroomId}");

            var submissions = new List<dynamic>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                submissions = JsonSerializer.Deserialize<List<dynamic>>(jsonString) ?? new List<dynamic>();
            }

            ViewBag.ClassroomId = classroomId;
            return View(submissions);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubmission(int id, int classroomId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"api/Submissions/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Submission deleted. The student can now reattempt the assignment.";
            }
            return RedirectToAction("ClassroomSubmissions", new { classroomId = classroomId });
        }

        [HttpGet]
        public async Task<IActionResult> EnrolledStudents(int classroomId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"api/Classrooms/classroom/{classroomId}/students");
            var students = new List<dynamic>();
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                students = JsonSerializer.Deserialize<List<dynamic>>(jsonString) ?? new List<dynamic>();
            }
            ViewBag.ClassroomId = classroomId;
            return View(students);
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
                    return View(submission);
                }
            }
            TempData["ErrorMessage"] = "Could not load submission details.";
            return RedirectToAction("ClassroomSubmissions", "Teacher");
        }
    }
}