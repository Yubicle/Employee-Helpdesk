using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;

namespace HelpdeskViewModels
{
    public class ProblemViewModel
    {
        private readonly ProblemDAO _dao;

        public int Id { get; set; }
        public string? Description { get; set; }

        public string? Timer { get; set; }


        public ProblemViewModel()
        {
            _dao = new ProblemDAO();
        }

        public async Task<List<ProblemViewModel>> GetAll()
        {
            List<ProblemViewModel> allVms = new();

            try
            {
                List<Problem> problems = await _dao.GetAll();

                foreach(Problem p in problems)
                {
                    ProblemViewModel pVm = new();

                    pVm.Id = p.Id;
                    pVm.Description = p.Description;
                    pVm.Timer = Convert.ToBase64String(p.Timer!);

                    allVms.Add(pVm);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return allVms;
        }

        public async Task GetByDescription()
        {
            try
            {
                Problem p = await _dao.GetByDescription(Description);
                Id = p.Id;
                Description = p.Description;
                Timer = Convert.ToBase64String(p.Timer!);
            }
            catch(NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Description = "not found";
            }
            catch(Exception ex)
            {
                Description = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
    }
}
