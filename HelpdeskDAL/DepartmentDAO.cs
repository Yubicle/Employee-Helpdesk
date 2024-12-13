using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskDAL {
    public class DepartmentDAO {
        private readonly IRepository<Department> _repo;

        public DepartmentDAO() {
            _repo = new HelpdeskRepository<Department>();
        }

        public async Task<List<Department>> GetAll() {
            return await _repo.GetAll();
        }
    }
}
