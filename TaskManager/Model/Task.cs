using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Model
{
    public class Task
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private User _owner;
        public User Owner 
        { 
            get { return _owner; } 
            set {  _owner = value; }
        }
        private Status _status;
        public Status Status
        {
            get { return _status; }
            set { _status= value; }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string _description;
        public string Description
        {
            get { return _description ?? throw new Exception("_description field was null"); }
            set { _description = value; }
        }
        private Category _scope;
        public Category Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
        private DateTime _since;
        public DateTime Since
        {
            get { return _since; }
            set { _since = value; }
        }
        private DateTime _deadline;
        public DateTime Deadline
        {
            get { return _deadline; }
            set { _deadline = value; }
        }
    }
    public class Status
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
