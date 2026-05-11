using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Model
{
    public class LoggingRecord
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _action;
        public string Action 
        { 
            set { _action = value; }
            get { return _action; }
        }
        private User _user;
        public User User 
        { 
            set { _user = value; }
            get { return _user; }
        }
        private DateTime _time;
        public DateTime Time 
        { 
            set { _time = value; }
            get { return _time; }
        }
    }
}
