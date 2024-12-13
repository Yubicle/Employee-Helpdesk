using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;

namespace HelpdeskViewModels
{
    public class CallViewModel
    {
        private readonly CallDAO _dao;
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProblemId { get; set; }
        public string? EmployeeName { get; set; }
        public string? ProblemDescription { get; set; }
        public string? TechName { get; set; }
        public int TechId { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public bool OpenStatus { get; set; }
        public string? Notes { get; set; }
        public string? Timer { get; set; }

        public CallViewModel()
        {
            _dao = new CallDAO();
        }

        public async Task GetById()
        {
            try
            {
                ProblemDAO pdao = new();
                EmployeeDAO edao = new();
                Call call = await _dao.GetById((int)Id!);
                EmployeeId = call.EmployeeId;
                ProblemId = call.ProblemId;

                Employee e = await edao.GetById(EmployeeId);
                Problem p = await pdao.GetById(ProblemId);

                EmployeeName = e.LastName;
                ProblemDescription = p.Description;
                TechId = call.TechId;

                // Get the TechName
                e = await edao.GetById(TechId);
                TechName = e.LastName;
                DateOpened = call.DateOpened;
                DateClosed = call.DateClosed;
                OpenStatus = call.OpenStatus;
                Notes = call.Notes;
                Timer = Convert.ToBase64String(call.Timer!);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Notes = "not found";
            }
            catch (Exception ex)
            {
                Notes = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<List<CallViewModel>> GetAll()
        {
            List<CallViewModel> allCVMs = new();
            try
            {
                ProblemDAO pdao = new();
                EmployeeDAO edao = new();

                List<Call> allCalls = await _dao.GetAll();

                foreach(Call c in allCalls)
                {
                    CallViewModel cVM = new();

                    cVM.Id = c.Id;
                    cVM.EmployeeId = c.EmployeeId;
                    Employee e = await edao.GetById(cVM.EmployeeId);
                    cVM.EmployeeName = e.LastName;

                    cVM.ProblemId = c.ProblemId;
                    Problem p = await pdao.GetById(cVM.ProblemId);
                    cVM.ProblemDescription = p.Description;
                    cVM.TechId = c.TechId;
                    e = await edao.GetById(cVM.TechId);
                    cVM.TechName = e.LastName;
                    cVM.DateOpened = c.DateOpened;
                    cVM.DateClosed = c.DateClosed;
                    cVM.OpenStatus = c.OpenStatus;
                    cVM.Notes = c.Notes;
                    cVM.Timer = Convert.ToBase64String(c.Timer!);

                    allCVMs.Add(cVM);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allCVMs;
        }

        public async Task Add()
        {

            Id = -1;
            try
            {
                Call call = new();
                call.EmployeeId = EmployeeId;
                call.ProblemId = ProblemId;
                call.TechId = TechId;
                call.DateOpened = DateOpened;
                call.DateClosed = DateClosed;
                call.OpenStatus = OpenStatus;
                call.Notes = Notes!;
                Id = await _dao.Add(call);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                Call call = new();
                call.Id = Id;
                call.EmployeeId = EmployeeId;
                call.ProblemId = ProblemId;
                call.TechId = TechId;
                call.DateOpened = DateOpened;
                call.DateClosed = DateClosed;
                call.OpenStatus = OpenStatus;
                call.Notes = Notes!;

                if (Timer is not null)
                {
                    call.Timer = Convert.FromBase64String(Timer!);
                }

                updateStatus = -1;
                updateStatus = Convert.ToInt16(await _dao.Update(call));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task<int> Delete()
        {
            try
            {
                // dao will return # of rows deleted
                return await _dao.Delete(Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }



    }

    
}
