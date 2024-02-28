using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TestTask.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PaientController : ControllerBase
    {
        private readonly ILogger<PaientController> _logger;
        private readonly string[] _patientNames = new[] { "Ali", "Hamza", "Ahmad", "Hassan" }; // Sample patient names

        public PaientController(ILogger<PaientController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{name}", Name = "GetPatient")]
        public IActionResult Get(string name)
        {
            // Get the user's role from the claims
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (userRole == "Doctor")
            {
                // If the user is a doctor, return patient data for the specified name
                var patientData = new Patients
                {
                    Date = DateTime.Now,
                    TemperatureC = Random.Shared.Next(-20, 55),
                    PatientName = name
                };
                return Ok(patientData);
            }
            else if (userRole == "Patient" && User.Identity.Name == name)
            {
                // If the user is a patient and the requested name matches their own, return their own data
                var patientData = new Patients
                {
                    Date = DateTime.Now,
                    TemperatureC = Random.Shared.Next(-20, 55),
                    PatientName = name
                };
                return Ok(patientData);
            }
            else
            {
                // Handle other roles, unauthorized access, or incorrect name parameter
                return Unauthorized();
            }
        }
    }
}
