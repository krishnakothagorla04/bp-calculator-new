using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BPCalculator; // Add this

namespace BPCalculator.Pages
{
    public class BloodPressureModel : PageModel
    {
        private readonly ILogger<BloodPressureModel> _logger;

        public BloodPressureModel(ILogger<BloodPressureModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public BloodPressure BP { get; set; }

        public string Category { get; set; }

        public void OnGet()
        {
            BP = new BloodPressure() { Systolic = 100, Diastolic = 60 };
        }

        public IActionResult OnPost()
        {
            // Validate systolic always > diastolic
            if (!(BP.Systolic > BP.Diastolic))
            {
                ModelState.AddModelError("", "Systolic must be greater than Diastolic");
                return Page();
            }

            // Compute category - Now using the enum-based property
            Category = BP.Category.ToString();

            // Telemetry logging
            _logger.LogInformation(
                "Blood Pressure reading {Systolic}/{Diastolic} categorized as {Category}",
                BP.Systolic, BP.Diastolic, Category
            );

            return Page();
        }
    }

    // REMOVE the duplicate BloodPressure class from here
}