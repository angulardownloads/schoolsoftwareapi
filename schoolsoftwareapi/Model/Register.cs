using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace schoolsoftwareapi.Model
{
    public class Register
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Usertype { get; set; }
        public string Accounttype { get; set; }
        public string License { get; set; }
        public string Registereddate { get; set; }
        public string Lastloggedin { get; set; }
    }
}
