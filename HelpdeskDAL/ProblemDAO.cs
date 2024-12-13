using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        private readonly IRepository<Problem> _repo;
        public ProblemDAO()
        {
            _repo = new HelpdeskRepository<Problem>();
        }

        public async Task<Problem> GetById(int? id)
        {
            return (await _repo.GetOne(p => p.Id == id))!;
        }

        public async Task<List<Problem>> GetAll()
        {
            return (await _repo.GetAll());
        }

        public async Task<Problem> GetByDescription(string? desc)
        {
            return (await _repo.GetOne(prb => prb.Description == desc))!;
        }

    }
}
