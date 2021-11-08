using schoolsoftwareapi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace schoolsoftwareapi.Abstract
{
  public interface IRegisterRepository
    {
        Task<Register> GetByID(int id);
        Task<IEnumerable<Register>> GetUserType(string username);
        Task<IEnumerable<Register>> GetRegister();

        string Add(Register Emp);
        void Delete(int id);
        void Update(Register Emp);
    }
}
