using Microsoft.AspNetCore.Mvc;
using schoolsoftwareapi.Abstract;
using schoolsoftwareapi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace schoolsoftwareapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        public readonly IRegisterRepository _RegisterRepo;
        public RegisterController(IRegisterRepository RegisterRepo)
        {
            _RegisterRepo = RegisterRepo;
        }
        // GET: api/<RegisterController>
        [HttpGet]
        public async Task<IEnumerable<Register>> GetRegister()
        {
            return await _RegisterRepo.GetRegister();
        }

        [HttpPost("{username}")]
        public async Task<IEnumerable<Register>> GetUserType(string username)
        {
            return await _RegisterRepo.GetUserType(username);
        }
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        //[HttpGet("{username}")]
        //public string GetUserType(string username)
        //{
        // var val= _RegisterRepo.GetUserType(username);
        //  return val.ToString();
        //}

        [HttpPost]
        public string Post([FromBody] Register emp)
        {
            if (ModelState.IsValid) ;
            return _RegisterRepo.Add(emp);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _RegisterRepo.Delete(id);
        }
    }
}
