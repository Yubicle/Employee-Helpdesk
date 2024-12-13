using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;

namespace HelpdeskViewModels {
    public class DepartmentViewModel {
        readonly private DepartmentDAO _dao;
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? Timer { get; set; }

        public DepartmentViewModel() {
            _dao = new DepartmentDAO();
        }


        public async Task<List<DepartmentViewModel>> GetAll() {
            List<DepartmentViewModel> allVms = new();
            try {
                List<Department> allDepartments = await _dao.GetAll();

                // we need to convert Employee instance to EmployeeViewModel because
                // the Web Layer isn't aware of the Domain class Employee

                foreach (Department dept in allDepartments) {
                    DepartmentViewModel deptVm = new();
                    deptVm.Id = dept.Id;
                    deptVm.Name = dept.DepartmentName;

                    //Binary value needs to be stored on client as base64
                    deptVm.Timer = Convert.ToBase64String(dept.Timer!);
                    allVms.Add(deptVm);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }
    }
}
