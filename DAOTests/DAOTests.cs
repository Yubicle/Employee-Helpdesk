using ExercisesDAL;
using HelpdeskDAL;
using HelpdeskViewModels;
using Xunit.Abstractions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CasestudyTests {
    public class DAOTests {

        private readonly ITestOutputHelper output;
        public DAOTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task Employee_GetByEmailTest() {
            EmployeeDAO dao = new();
            Employee selectedEmployee = await dao.GetByEmail("bs@abc.com");
            Assert.NotNull(selectedEmployee);
        }

        [Fact]
        public async Task Employee_LoadPicsTest()
        {
            {
                PicsUtility util = new();
                Assert.True(await util.AddEmployeePicsToDb());
            }
        }
        [Fact]
        public async Task Employee_ComprehensiveTest()
        {
            EmployeeDAO dao = new();
            Employee newEmployee = new()
            {
                FirstName = "Joe",
                LastName = "Smith",
                PhoneNo = "(555)555-1234",
                Title = "Mr.",
                DepartmentId = 100,
                Email = "js@abc.com"
            };
            int newEmployeeId = await dao.Add(newEmployee);
            output.WriteLine("New Employee Generated - Id = " + newEmployeeId);
            newEmployee = await dao.GetById(newEmployeeId);
            byte[] oldtimer = newEmployee.Timer!;
            output.WriteLine("New Employee " + newEmployee.Id + " Retrieved");
            newEmployee.PhoneNo = "(555)555-1233";
            if (await dao.Update(newEmployee) == UpdateStatus.Ok)
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was updated to - " + newEmployee.PhoneNo);
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " phone# was not updated!");
            }
            newEmployee.Timer = oldtimer; // to simulate another user
            newEmployee.PhoneNo = "doesn't matter data is stale now";
            if (await dao.Update(newEmployee) == UpdateStatus.Stale)
            {
                output.WriteLine("Employee " + newEmployeeId + " was not updated due to stale data");
            }

            dao = new();
            await dao.GetById(newEmployeeId);
            if (await dao.Delete(newEmployeeId) == 1)
            {
                output.WriteLine("Employee " + newEmployeeId + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + newEmployeeId + " was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await dao.GetById(newEmployeeId));
        }

        [Fact]
        public async Task Call_ComprehensiveTest()
        {
            CallDAO cdao = new();
            Call newCall = new();
            ProblemDAO pdao = new();
            Problem badDrive = await pdao.GetByDescription("Hard Drive Failure");
            EmployeeDAO edao = new();
            Employee tech = await edao.GetByLastName("Burner");
            Employee me = await edao.GetByLastName("Alokwe");
            newCall.EmployeeId = me.Id;
            newCall.ProblemId = badDrive.Id;
            newCall.TechId = tech.Id;
            newCall.DateOpened = DateTime.Now;
            newCall.DateClosed = null;
            newCall.OpenStatus = true;
            newCall.Notes = $"{me.LastName}’s drive is shot, {tech.LastName} to fix it";
            int newCallId = await cdao.Add(newCall);
            output.WriteLine($"New Call Generated - Id = {newCallId}");

            newCall = await cdao.GetById(newCallId);
            byte[] oldtimer = newCall.Timer!;
            newCall.Notes = $"{newCall.Notes} \n Ordered new drive!";
            if (await cdao.Update(newCall) == UpdateStatus.Ok)
            {
                output.WriteLine($"Call was updated {newCall.Notes}");
            }
            else
            {
                output.WriteLine("Call was not updated due to stale data");
            }
            newCall.Timer = oldtimer; // to simulate another user
            newCall.Notes = "doesn't matter data is stale now";
            if (await cdao.Update(newCall) == UpdateStatus.Stale)
            {
                output.WriteLine("Call was not updated data was stale");
            }

            cdao = new();
            await cdao.GetById(newCallId); ;
            if (await cdao.Delete(newCallId) == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else
            {
                output.WriteLine("Call was not deleted");
            }
            // should be null because it was just deleted
            Assert.Null(await cdao.GetById(newCallId));
        }

        [Fact]
        public async Task Employee_ComprehensiveVMTest()
        {
            EmployeeViewModel evm = new()
            {
                Title = "Mr.",
                Firstname = "Some",
                Lastname = "Employee",
                Email = "some@abc.com",
                Phoneno = "(777)777-7777",
                DepartmentId = 100 // ensure department id is in Departments table
            };
            await evm.Add();
            output.WriteLine("New Employee Added - Id = " + evm.Id);
            int? id = evm.Id; // need id for delete later
            await evm.GetById();
            output.WriteLine("New Employee " + id + " Retrieved");
            evm.Phoneno = "(555)555-1233";
            if (await evm.Update() == 1)
            {
                output.WriteLine("Employee " + id + " phone# was updated to - " +
               evm.Phoneno);
            }
            else
            {
                output.WriteLine("Employee " + id + " phone# was not updated!");
            }
            evm.Phoneno = "Another change that should not work";
            if (await evm.Update() == -2)
            {
                output.WriteLine("Employee " + id + " was not updated due to stale data");
            }
            evm = new EmployeeViewModel
            {
                Id = id
            };
            // need to reset because of concurrency error
            await evm.GetById();
            if (await evm.Delete() == 1)
            {
                output.WriteLine("Employee " + id + " was deleted!");
            }
            else
            {
                output.WriteLine("Employee " + id + " was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async ()
           => await evm.GetById());
        }

        [Fact]
        public async Task Call_ComprehensiveVMTest()
        {
            EmployeeDAO edao = new(); 
            ProblemDAO pdao = new();

            Employee me = await edao.GetByLastName("Alokwe");
            Employee tech = await edao.GetByLastName("Burner");      
            Problem problem = await pdao.GetByDescription("Memory Upgrade");

            CallViewModel cvm = new();
            cvm.EmployeeId = me.Id;
            cvm.ProblemId = problem.Id;
            cvm.TechId = tech.Id;
            cvm.DateOpened = DateTime.Now;
            cvm.DateClosed = null;
            cvm.OpenStatus = true;
            cvm.Notes = $"{me.LastName} has bad RAM, {tech.LastName} to fix it";

            await cvm.Add();
            output.WriteLine($"New Call Generated - Id = {cvm.Id}");
            int? id = cvm.Id; // need id for delete later
            await cvm.GetById();

            cvm.Notes = $"{cvm.Notes}\n Ordered new RAM!";
            if (await cvm.Update() == 1)
            {
                output.WriteLine($"Call was updated {cvm.Notes}");
            }
            else
            {
                output.WriteLine("Call was not updated");
            }
            cvm.Notes = "Another change that should not work";
            if (await cvm.Update() == -2)
            {
                output.WriteLine("Call was not updated data was stale");
            }
            cvm = new CallViewModel();
            cvm.Id = (int)id;

            // need to reset because of concurrency error
            await cvm.GetById();
            if (await cvm.Delete() == 1)
            {
                output.WriteLine("Call was deleted!");
            }
            else
            {
                output.WriteLine("Call was not deleted");
            }
            // should throw expected exception
            Task<NullReferenceException> ex = Assert.ThrowsAsync<NullReferenceException>(async () 
                => await cvm.GetById());
        }

    }

}