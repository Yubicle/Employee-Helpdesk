using HelpdeskDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskViewModels {
    public class EmployeeViewModel {
        private readonly EmployeeDAO _dao;
        public string? Title { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Phoneno { get; set; }
        public string? Timer { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? Id { get; set; }
        public bool? IsTech { get; set; }
        public string? StaffPicture64 { get; set; }
        // constructor
        public EmployeeViewModel() {
            _dao = new EmployeeDAO();
        }


        public async Task GetByEmail() {
            try {
                Employee emp = await _dao.GetByEmail(Email);
                Title = emp.Title;
                Firstname = emp.FirstName;
                Lastname = emp.LastName;
                Email = emp.Email;
                Phoneno = emp.PhoneNo;
                Id = emp.Id;
                DepartmentId = emp.DepartmentId;
                DepartmentName = emp.Department.DepartmentName;
                IsTech = emp.IsTech ?? false;
                if (emp.StaffPicture != null) {
                    StaffPicture64 = Convert.ToBase64String(emp.StaffPicture);
                }
                Timer = Convert.ToBase64String(emp.Timer!);

            }
            catch (NullReferenceException nex) {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex) {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetById() {
            try {
                Employee emp = await _dao.GetById((int)Id!);
                Title = emp.Title;
                Firstname = emp.FirstName;
                Lastname = emp.LastName;
                Email = emp.Email;
                Phoneno = emp.PhoneNo;
                Id = emp.Id;
                DepartmentId = emp.DepartmentId;
                DepartmentName = emp.Department.DepartmentName;
                IsTech = emp.IsTech ?? false;
                if (emp.StaffPicture != null) {
                    StaffPicture64 = Convert.ToBase64String(emp.StaffPicture);
                }

                Timer = Convert.ToBase64String(emp.Timer!);
            }
            catch (NullReferenceException nex) {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex) {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetByPhoneNumber() {
            try {
                Employee emp = await _dao.GetByPhoneNumber(Phoneno);
                Title = emp.Title;
                Firstname = emp.FirstName;
                Lastname = emp.LastName;
                Email = emp.Email;
                Phoneno = emp.PhoneNo;
                Id = emp.Id;
                DepartmentId = emp.DepartmentId;
                DepartmentName = emp.Department.DepartmentName;
                IsTech = emp.IsTech ?? false;
                if (emp.StaffPicture != null) {
                    StaffPicture64 = Convert.ToBase64String(emp.StaffPicture);
                }
                Timer = Convert.ToBase64String(emp.Timer!);
            }
            catch (NullReferenceException nex) {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex) {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetByLastName()
        {
            try
            {
                Employee emp = await _dao.GetByLastName(Lastname);
                Title = emp.Title;
                Firstname = emp.FirstName;
                Lastname = emp.LastName;
                Email = emp.Email;
                Phoneno = emp.PhoneNo;
                Id = emp.Id;
                DepartmentId = emp.DepartmentId;
                DepartmentName = emp.Department.DepartmentName;
                IsTech = emp.IsTech ?? false;
                if (emp.StaffPicture != null)
                {
                    StaffPicture64 = Convert.ToBase64String(emp.StaffPicture);
                }
                Timer = Convert.ToBase64String(emp.Timer!);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Lastname = "not found";
            }
            catch (Exception ex)
            {
                Lastname = "not found";
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<List<EmployeeViewModel>> GetAll() {
            List<EmployeeViewModel> allVms = new();
            try {
                List<Employee> allEmployees = await _dao.GetAll();

                foreach (Employee emp in allEmployees) {
                    EmployeeViewModel empVm = new();
                    empVm.Title = emp.Title;
                    empVm.Firstname = emp.FirstName;
                    empVm.Lastname = emp.LastName;
                    empVm.Phoneno = emp.PhoneNo;
                    empVm.Email = emp.Email;
                    empVm.Id = emp.Id;
                    empVm.DepartmentId = emp.DepartmentId;
                    empVm.DepartmentName = emp.Department.DepartmentName;

                    // No compiler warning but used null-coalescing as a safeguard.
                    empVm.IsTech = emp.IsTech ?? false;

                    if (emp.StaffPicture != null) {
                        empVm.StaffPicture64 = Convert.ToBase64String(emp.StaffPicture);
                    }
                    empVm.Timer = Convert.ToBase64String(emp.Timer!);
                    allVms.Add(empVm);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allVms;
        }

        public async Task Add() {
            Id = -1;
            try {
                Employee emp = new();
                emp.Title = Title;
                emp.FirstName = Firstname;
                emp.LastName = Lastname;
                emp.PhoneNo = Phoneno;
                emp.Email = Email;
                emp.DepartmentId = DepartmentId;
                if (StaffPicture64 is not null)
                {
                    emp.StaffPicture = Convert.FromBase64String(StaffPicture64!);
                }
                emp.IsTech = IsTech ?? false;
                Id = await _dao.Add(emp);
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int> Update() {
            int updateStatus;
            try {
                Employee emp = new();
                emp.Title = Title;
                emp.FirstName = Firstname;
                emp.LastName = Lastname;
                emp.PhoneNo = Phoneno;
                emp.Email = Email;
                emp.Id = (int)Id!;
                emp.DepartmentId = DepartmentId;

                if (StaffPicture64 is not null)
                {
                    emp.StaffPicture = Convert.FromBase64String(StaffPicture64!);
                }
                
                if (Timer is not null)
                {
                    emp.Timer = Convert.FromBase64String(Timer!);
                }
                

                updateStatus = -1;
                updateStatus = Convert.ToInt16(await _dao.Update(emp)); // overwrite status
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task<int> Delete() {
            try {
                // dao will return number of rows deleted
                return await _dao.Delete(Id);
            }
            catch (Exception ex) {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

    }
}
