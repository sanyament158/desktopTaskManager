using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    internal class ViewResponsibilitiesViewModel: INotifyPropertyChanged
    {
        public ViewResponsibilitiesViewModel(User _enteredUser) 
        {
            this._enteredUser = _enteredUser;
            _ = InitializeAsync();
        }
        private User _enteredUser;

        private List<Model.Task> _tasks;
        public List<Model.Task> Tasks 
        {
            get { return _tasks; }
            set { _tasks = value;
                OnPropertyChanged();
            }
        }
        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                onUserChanged();
            }
        }
        private string _ratio;
        public string Ratio
        {
            get { return _ratio; }
            set { _ratio = value;
            OnPropertyChanged();}
        }
        private List<User> _users;
        public List<User> Users
        {
            get { return _users; }
            set { _users = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(
                (obj) =>
                {
                    MainFrame.mainFrame.GoBack();
                })
                );
            }
        }
        private async void onUserChanged()
        {
            List<Model.Task> allTasks = await DataBaseService.GetTasks();
            allTasks = allTasks.Where(t => t.Status.Id == 1 || t.Status.Id == 3).ToList();

            List<Model.Task> sortedTasks = new List<Model.Task>();
            foreach ( Model.Task t in allTasks )
            {
                if (t.IdUserTaked == SelectedUser.Id)
                {
                    sortedTasks.Add(t);
                }
            }
            Tasks = sortedTasks;
            Ratio = $"{sortedTasks.Count()} задач(-и) на одного сотрудника";
        }
        private async System.Threading.Tasks.Task InitializeAsync()
        {
            List<Model.Task> tasks = await DataBaseService.GetTasks();
            List<User> allUsers = await DataBaseService.GetUsers();
            Users = allUsers.Where(u => u.Role.Id == 2).ToList();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
    }
}
