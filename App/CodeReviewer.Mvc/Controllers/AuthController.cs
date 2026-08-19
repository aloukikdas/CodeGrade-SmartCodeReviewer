using CodeReviewer.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CodeReviewer.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonPayload = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Auth/register", jsonPayload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Account created successfully! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Email might already be in use.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonPayload = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Auth/login", jsonPayload);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<JsonElement>(jsonString);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.GetProperty("id").ToString()),
                    new Claim(ClaimTypes.Name, user.GetProperty("name").GetString() ?? ""),
                    new Claim(ClaimTypes.Email, user.GetProperty("email").GetString() ?? ""),
                    new Claim(ClaimTypes.Role, user.GetProperty("role").GetString() ?? "")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                var role = user.GetProperty("role").GetString();
                if (role == "Teacher")
                {
                    return RedirectToAction("Index", "Teacher");
                }
                return RedirectToAction("Dashboard", "Student");
            }
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}