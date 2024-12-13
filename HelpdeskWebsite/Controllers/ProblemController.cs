using HelpdeskViewModels;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                ProblemViewModel viewmodel = new();
                List<ProblemViewModel> allProblems= await viewmodel.GetAll();
                return Ok(allProblems);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " +
                                ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Something went wrong
            }
        }
    }
}
