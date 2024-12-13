using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL {
    public class EmployeeDAO
    {

        private readonly IRepository<Employee> _repo;
        public EmployeeDAO() 
        {
            _repo = new HelpdeskRepository<Employee>();
        }
        public async Task<Employee> GetByEmail(string? email)
        {
            return (await _repo.GetOne(emp => emp.Email == email))!;
        }

        public async Task<Employee> GetById(int id)
        {
            return (await _repo.GetOne(emp => emp.Id == id))!;
        }
        public async Task<List<Employee>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<int> Add(Employee newEmployee)
        {
            return (await _repo.Add(newEmployee)).Id;
        }

        public async Task<Employee> GetByPhoneNumber(string? phoneNumber)
        {
            return (await _repo.GetOne(emp => emp.PhoneNo == phoneNumber))!;
        }

        public async Task<Employee> GetByLastName(string? lastName)
        {
            return (await _repo.GetOne(emp => emp.LastName == lastName))!;
        }

        public async Task<UpdateStatus> Update(Employee updatedEmployee) {
            return await _repo.Update(updatedEmployee);
        }

        public async Task<int> Delete(int? id)
        {
            return await _repo.Delete((int)id!);
        }
    }
}
