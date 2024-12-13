using System.Diagnostics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskViewModels;

namespace HelpdeskWebsite.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try {
                DepartmentViewModel viewModel = new();
                List<DepartmentViewModel> allDepartments = await viewModel.GetAll();
                return Ok(allDepartments);
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }
    }
}
