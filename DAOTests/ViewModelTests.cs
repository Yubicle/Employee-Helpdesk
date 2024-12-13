using HelpdeskViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpdeskDAL;


namespace CasestudyTests {
    public class ViewModelTests {

        [Fact]
        public async Task Employee_GetByEmail()
        {
            EmployeeViewModel vm = new() { Email = "miracle@abc.com" };
            await vm.GetByEmail();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetByIdTest() {
            EmployeeViewModel vm = new() { Id = 1 };
            await vm.GetById();
            Assert.NotNull(vm.Firstname);
        }

        [Fact]
        public async Task Employee_GetAllTest() {
            List<EmployeeViewModel> allEmployeeVms;
            EmployeeViewModel vm = new();
            allEmployeeVms = await vm.GetAll();
            Assert.True(allEmployeeVms.Count > 0);
        }

        [Fact]
        public async Task Employee_AddTest() {
            EmployeeViewModel vm = new();
            vm.Title = "Mr.";
            vm.Firstname = "Miracle";
            vm.Lastname = "Alokwe";
            vm.Phoneno = "(777)777-7777";
            vm.Email = "miracle@abc.com";
            vm.DepartmentId = 100;
            
            vm.IsTech = false;

            await vm.Add();
            Assert.True(vm.Id > 0);
        }

        [Fact]
        public async Task Employee_UpdateTest() {
            EmployeeViewModel vm = new() { Email = "miracle@abc.com" };
            await vm.GetByEmail(); // Employee just added in Add test
            vm.Email = vm.Email == "(555)555-5551" ? "(555)555-5552" : "(555)555-5556";

            Assert.True(await vm.Update() == 1);
        }

        [Fact]
        public async Task Employee_DeleteTest() {
            EmployeeViewModel vm = new() { Phoneno = "(233)543-1234" };
            await vm.GetByPhoneNumber(); // Employee just added
            Assert.True(await vm.Delete() == 1); // 1 employee deleted
        }

        [Fact]
        public async Task Employee_GetByPhoneNumberTest() {
            EmployeeViewModel vm = new() { Phoneno = "(555)555-5552" };
            await vm.GetByPhoneNumber();
            Assert.NotNull(vm.Phoneno);
        }

        [Fact]
        public async Task Employee_ConcurrencyTest()
        {
            EmployeeViewModel vm1 = new() { Email = "miracle@abc.com" };
            EmployeeViewModel vm2 = new() { Email = "miracle@abc.com" };
            await vm1.GetByEmail(); // Fetch same employee to simulate 2 users
            if (vm1.Email != "Not Found") // make sure we found an Employee
            {
                await vm2.GetByEmail(); // fetch some data
                vm1.Phoneno = vm1.Phoneno == "(555)555-5551" ? "(777)777-7777" : "(233)543-1234";
                if (await vm1.Update() == 1)
                {
                    vm2.Phoneno = "(111)111-1111"; // just need any value
                    Assert.True(await vm2.Update() == -2);
                }
            }
            else
                Assert.True(false); // Employee not found
        }

    }
}
