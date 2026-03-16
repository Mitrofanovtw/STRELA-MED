using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRELA_MED.Services
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }

    public static class AuthService
    {
        public static User Authenticate(string login, string password)
        {
            if (login == "admin" && password == "admin123")
                return new User { Login = "admin", Role = "admin", UserId = 0 };
            if (login == "doctor" && password == "doctor123")
                return new User { Login = "doctor", Role = "doctor", UserId = 101 };
            if (login == "user" && password == "user123")
                return new User { Login = "user", Role = "user", UserId = 202 };

            return null;
        }
    }
}
