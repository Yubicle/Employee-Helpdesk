
using HelpdeskWebsite.Reports;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskWebsite.Controllers
{
    public class ReportController : Controller
    {

        private readonly IWebHostEnvironment _env;

        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [Route("api/employeereport")]
        [HttpGet]
        public IActionResult GetEmployeeReport()
        {
            EmployeeReport rep = new();
            rep.GenerateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }

        [Route("api/callreport")]
        [HttpGet]
        public IActionResult GetCallReport()
        {
            CallReport rep = new();
            rep.GenerateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }

    }
}
