using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.Infrastucture.Navigation;
using System.Windows;
using TaskManager.View.Pages.Users;

namespace TaskManager.ViewModel.Pages.Users
{
    class MyTasksPageViewModel: INotifyPropertyChanged 
    {
        public MyTasksPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            _ = InitializeAsync();
        }

        // fields && properties
        private User _enteredUser;
        private List<Model.Task> _tasks = new List<Model.Task>();
        public List<Model.Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value;
                OnPropertyChanged();
            }
        }
        private Model.Task _selectedTask;
        public Model.Task SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // commands
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand((obj) =>
            {
                MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));

            } ) ); }
        }
        private RelayCommand _goToShowDetailsCommand;
        public RelayCommand GoToShowDetailsCommand
        {
            get
            {
                return _goToShowDetailsCommand ?? (
                    _goToShowDetailsCommand = new RelayCommand(
                            (obj) =>
                            {
                                if (obj is Model.Task task)
                                {
                                    if (_enteredUser.Id == task.Owner.Id) MainFrame.mainFrame.Navigate(new ShowTaskDetailsPage(_enteredUser, task, "owner"));
                                    else MainFrame.mainFrame.Navigate(new ShowTaskDetailsPage(_enteredUser, task, "user"));
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка!");
                                }
                            },
                            (obj) => (SelectedTask != null && SelectedTask.Id != 0)
                        )
                    );
            }
        }
        private async System.Threading.Tasks.Task InitializeAsync()
        {
            List<Model.Task> allTasks = await DataBaseService.GetTasks();
            List<Model.Task> filteredTasks = allTasks.Where(t => t.Owner.Id == _enteredUser.Id).Where(t => t.Status.Id != 2).ToList();
            Tasks = filteredTasks;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
