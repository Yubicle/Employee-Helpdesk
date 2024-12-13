using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class CallDAO
    {
        private readonly IRepository<Call> _repo;

        public CallDAO()
        {
            _repo = new HelpdeskRepository<Call>();
        }

        public async Task<Call> GetById(int id)
        {
            return (await _repo.GetOne(call => call.Id == id))!;
        }

        public async Task<List<Call>> GetByEmployeeId(int empId)
        {
            return await _repo.GetSome(call => call.EmployeeId == empId);
        }

        public async Task<List<Call>> GetByProblemId(int problemId)
        {
            return await _repo.GetSome(call => call.ProblemId == problemId);
        }

        public async Task<List<Call>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<int> Add(Call newCall)
        {
            return (await _repo.Add(newCall)).Id;
        }

        public async Task<UpdateStatus> Update(Call updatedCall)
        {
            return await _repo.Update(updatedCall);
        }

        public async Task<int> Delete(int? id)
        {
            return await _repo.Delete((int)id!);
        }



    }
}
