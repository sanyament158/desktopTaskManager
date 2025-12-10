using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;

namespace TaskManager.Model
{
    public class Category
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
            set { _name = value; }
            get { return _name; } 
        }

        
    }
}
