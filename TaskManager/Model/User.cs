using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.RightsManagement;
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
        private Role role;
        public Role Role
        {
            get { return role; }
            set { role = value; }
        }
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
    public class Role
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
