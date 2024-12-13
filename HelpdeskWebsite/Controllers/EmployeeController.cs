using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using HelpdeskViewModels;


namespace HelpdeskWebsite.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase {
        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email) {
            try {
                EmployeeViewModel viewmodel = new() { Email = email };
                await viewmodel.GetByEmail();
                return Ok(viewmodel);
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " +
                                ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Something went wrong
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(EmployeeViewModel viewmodel) {
            try {
                int returnVal = await viewmodel.Update();
                return returnVal switch {
                    1 => Ok(new { msg = "Employee " + viewmodel.Lastname + " updated!" }),
                    -1 => Ok(new { msg = "Employee " + viewmodel.Lastname + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + viewmodel.Lastname + ", Employee not updated!" }),
                    _ => Ok(new { msg = "Employee " + viewmodel.Lastname + " not updated!" }),
                };
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try {
                EmployeeViewModel viewModel = new();
                List<EmployeeViewModel> allEmployees = await viewModel.GetAll();
                return Ok(allEmployees);
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(EmployeeViewModel viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 1
                    ? Ok(new { msg = "Employee " + viewModel.Lastname + " added!" })
                    : Ok(new { msg = "Employee " + viewModel.Lastname + " not added!" });
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) {
            try {
                EmployeeViewModel viewModel = new() { Id = id };
                return await viewModel.Delete() == 1 ?
                    Ok(new { msg = "Employee " + id + " deleted!" })
               : Ok(new { msg = "Employee " + id + " not deleted!" });
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

    }
}
