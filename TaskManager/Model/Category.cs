using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;

namespace TaskManager.Model
{
    public class Category
    {
        private string _name;
        public string Name 
        { 
            set { _name = value; }
            get { return _name; } 
        }
    }
}
