using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class UpdateTaskViewModel: INotifyPropertyChanged
    {
        //Fields & Properties
        private ObservableCollection<Category> _scopes;
        public ObservableCollection<Category> Scopes
        {
            get { return _scopes; }
            set
            {
                _scopes = value;
                OnPropertyChanged();
            }
        }
        private string _owner;
        public string Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                OnPropertyChanged();
            }
        }
        private int _idScope;
        public int IdScope
        {
            get { return _idScope; }
            set
            {
                _idScope = value;
                OnPropertyChanged();
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private DateTime _since;
        public string Since
        {
            get { return _since.ToString("yyyy-MM-dd"); }
            set
            {
                _since = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture
                );
                OnPropertyChanged();
            }
        }
        private DateTime _deadline;
        public string Deadline
        {
            get { return _deadline.ToString("yyyy-MM-dd"); }
            set
            {
                _deadline = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture
                );
                OnPropertyChanged();
            }
        }
        //Commands
        //Methods

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
