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
        private int _idOwner;
        public int IdOwner
        {
            get { return _idOwner; }
            set { _idOwner = value; } 
        }
        private int _idStatus;
        public int IdStatus
        {
            get { return _idStatus; }
            set { _idStatus= value; }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string? _description;
        public string Description
        {
            get { return _description ?? throw new Exception("_description field was null"); }
            set { _description = value; }
        }
        private int _idScope;
        public int IdScope
        {
            get { return _idScope; }
            set { _idScope = value; }
        }
        private DateTime _since;
        public DateTime Since
        {
            get { return _since; }
            set { _since = value; }
        }
        private DateTime _deadline;
        public DateTime Deadine
        {
            get { return _deadline; }
            set { _deadline = value; }
        }
    }

    public class TaskHumanReadable
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set {  _id = value; }
        }
        private string _owner;
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private string _importance;
        public string Importance
        {
            get { return _importance; }
            set { _importance = value; }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private string? _description;
        public string Description
        {
            get { return _description ?? throw new Exception("_description field was null"); }
            set { _description = value; }
        }
        private string _scope;
        public string Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
        private DateTime _since;
        public string Since
        {
            get { return _since.ToShortDateString(); }
            set
            {
                _since = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture
                );
            }
        }
        private DateTime _deadline;
        public string Deadline
        {
            get { return _deadline.ToShortDateString(); }
            set { _deadline = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture
                ); }
        }

    }
}
