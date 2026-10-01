using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HospitalManagementSystem.Models;
using System.Text.Json;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        // Dependency Injection for In-Memory Cache[cite: 1]
        private readonly IMemoryCache _memoryCache;
        // Dependency Injection for HTTP Client to call external REST APIs[cite: 1]
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor to inject services
        public DoctorController(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
        }

        // Helper method to simulate fetching static doctor records from database
        private List<Doctor> GetInitialDoctorsList()
        {
            return new List<Doctor>
            {
                new Doctor { DoctorId = 1, DoctorName = "Dr. Rajesh Sharma", Specialization = "Cardiology", Experience = 12, ConsultationFee = 800, ContactNumber = "9876789342" },
                new Doctor { DoctorId = 2, DoctorName = "Dr. Priya Patel", Specialization = "Dermatology", Experience = 8, ConsultationFee = 600, ContactNumber = "9067854311" },
                new Doctor { DoctorId = 3, DoctorName = "Dr. Amit Verma", Specialization = "Neurology", Experience = 15, ConsultationFee = 1000, ContactNumber = "8354678901" },
                new Doctor { DoctorId = 4, DoctorName = "Dr. Veeraj Waghela", Specialization = "Orthopedics", Experience = 5, ConsultationFee = 700, ContactNumber = "9408887017" }
            };
        }

        // 1. Doctor List Page - Demonstrates In-Memory Caching (3-minute expiration)[cite: 1]
        public IActionResult Index()
        {
            string cacheKey = "DoctorListCache";

            // Check if doctor list is available in IMemoryCache[cite: 1]
            if (!_memoryCache.TryGetValue(cacheKey, out List<Doctor>? doctors))
            {
                // Retrieve data if not in cache
                doctors = GetInitialDoctorsList();

                // Define caching options with absolute expiration of 3 minutes[cite: 1]
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                // Save doctor list into cache memory[cite: 1]
                _memoryCache.Set(cacheKey, doctors, cacheEntryOptions);
            }

            return View(doctors);
        }

        // 2. Doctor Details Page - Demonstrates Response Caching and Query String parameters[cite: 1]
        // Caches response on the client/server for 60 seconds[cite: 1]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Details(int doctorId) // DoctorId received via Query String[cite: 1]
        {
            // Retrieve full doctor list
            var doctors = GetInitialDoctorsList();

            // Find specific doctor matching DoctorId
            var doctor = doctors.FirstOrDefault(d => d.DoctorId == doctorId);

            if (doctor == null)
            {
                return NotFound();
            }

            // Store selected doctor's specialization into Session state[cite: 1]
            HttpContext.Session.SetString("SelectedSpecialization", doctor.Specialization);

            return View(doctor);
        }

        // 3. Appointment Form Page - Displays form with Hidden Field and Cookie storage[cite: 1]
        [HttpGet]
        public IActionResult BookAppointment(int doctorId)
        {
            // Retrieve specialization from Session state[cite: 1]
            ViewBag.SelectedSpecialization = HttpContext.Session.GetString("SelectedSpecialization");

            // Pass DoctorId to View to be stored in Hidden Field[cite: 1]
            ViewBag.DoctorId = doctorId;

            return View();
        }

        // POST Action for submitting Appointment Form
        [HttpPost]
        public IActionResult BookAppointment(int doctorId, string patientName)
        {
            // Create a Cookie to store Patient Name for 7 days[cite: 1]
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true
            };
            Response.Cookies.Append("PatientName", patientName, options);

            ViewBag.Message = $"Appointment booked successfully for {patientName} (Doctor ID: {doctorId})!";
            return View("Confirmation");
        }

        // 4. Asynchronous Web Request Page - Fetches external JSON posts asynchronously[cite: 1]
        public async Task<IActionResult> Posts()
        {
            List<Post>? postsList = new List<Post>();

            // Create client instance from factory[cite: 1]
            var client = _httpClientFactory.CreateClient();

            // Asynchronously fetch JSON posts from jsonplaceholder API[cite: 1]
            HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");

            if (response.IsSuccessStatusCode)
            {
                // Read HTTP content stream asynchronously
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize JSON string into List of Post objects
                postsList = JsonSerializer.Deserialize<List<Post>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Read stored patient name from Cookie if present[cite: 1]
            ViewBag.PatientName = Request.Cookies["PatientName"];

            return View(postsList);
        }
    }
}