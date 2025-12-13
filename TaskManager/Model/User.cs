using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace TaskManager.Model
{
    public class User
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public int IdRole { get; set; }
    }

    public class UserResponse
    {
        public string username { get; set; }
        public int idRole { get; set; }
    }
    public class UserRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
