using HelpdeskViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            try
            {
                CallViewModel viewmodel = new();
                List<CallViewModel> allCalls = await viewmodel.GetAll();
                return Ok(allCalls);
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

        [HttpPost]
        public async Task<ActionResult> Post(CallViewModel viewModel) {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 1
                    ? Ok(new { msg = "Call " + viewModel.Id+ " added!" })
                    : Ok(new { msg = "Call " + viewModel.Id+ " not added!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(CallViewModel viewmodel) {
            try
            {
                int returnVal = await viewmodel.Update();
                return returnVal switch
                {
                    1 => Ok(new { msg = "Call " + viewmodel.Id + " updated!" }),
                    -1 => Ok(new { msg = "Call " + viewmodel.Id + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + viewmodel.Id + ", Call not updated!" }),
                    _ => Ok(new { msg = "Call " + viewmodel.Id + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // Something went wrong
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CallViewModel viewModel = new() { Id = id };
                return await viewModel.Delete() == 1 ?
                    Ok(new { msg = "Call " + id + " deleted!" })
               : Ok(new { msg = "Call " + id + " not deleted!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }
    }
}
