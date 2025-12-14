using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewTaskPageViewModel: INotifyPropertyChanged
    {
        public AddNewTaskPageViewModel(User enteredUser) 
        {
            _enteredUser = enteredUser;
        }
        //Fields & Properties
        private User _enteredUser;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
