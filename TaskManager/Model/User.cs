using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace TaskManager.Model
{
    public class User
    {
        public int? id;
        public string username;
        public string? fname;
        public string? lname;
        public int idRole;
        public string password;
    }

    public class UserResponse
    {
        public string username;
        public int idRole;
    }
    public class UserRequest
    {
        public string username;
        public string password;
    }
}
