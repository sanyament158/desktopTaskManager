using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Users;

namespace TaskManager.ViewModel.Pages.Users
{
    public class ShowTaskDetailsPageViewModel: INotifyPropertyChanged
    {
        public ShowTaskDetailsPageViewModel(User enteredUser, Model.Task enteredTask, string source)
        {
            MessageBox.Show(source);

            /*
            may be: 
                user, 
                takedUser,
                finished,
                owner
             */
            switch (source)
            {
                case "owner":
                    {
                        switch (enteredTask.Status.Id)
                        {
                            case 1:
                                MarkAsFinishVisibility = Visibility.Visible;
                                break;
                            case 3:
                                IsCheckedVisibility = Visibility.Visible;
                                break;
                            case 4:
                                IsCheckedVisibility = Visibility.Visible;
                                break;
                        }
                        break;
                    }
                case "user":
                    {
                        switch (enteredTask.Status.Id)
                        {
                            case 1:
                                if (enteredTask.IdUserTaked == enteredUser.Id)
                                MarkAsFinishVisibility = Visibility.Visible;
                                break;
                            case 4:
                                IsWaitingVisibility = Visibility.Visible;
                                break;
                        }
                        break;
                    }
            } 

            _source = source;
            _enteredTask = enteredTask;
            _enteredUser = enteredUser;
            if (EnteredTask.IdUserTaked != null && EnteredTask.IdUserTaked != 0)
            {
                IsTaked = Visibility.Visible;
            } else IsTaked = Visibility.Collapsed;
            _ = InitializeAsync();

        }

        //Fields & Properties
        private readonly string _source;
        public Visibility IsTaked
        {
            get; set;
        }
        public string UserTakedText { get; set; }

        private User _enteredUser;
        public User EnteredUser
        {
            get { return _enteredUser; }
            set { _enteredUser = value;
                OnPropertyChanged();
            }
        }
        private Model.Task _enteredTask;
        public Model.Task EnteredTask
        {
            get { return  _enteredTask; }
            set { _enteredTask = value;
                OnPropertyChanged();
            }
        }
        private List<User> _users;
        public List<User> Users
        {
            set { _users = value;
                OnPropertyChanged();
            }
            get { return _users; }
        }
        private User? _selectedUser;
        public User? SelectedUser
        {
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
            get
            {
                return _selectedUser;
            }
        }

            //buttons visibility
        public Visibility TakeToUserVisibility { get; set; }    
        private Visibility _isWaitingVisibility = Visibility.Collapsed;
        public Visibility IsWaitingVisibility
        {
            get { return  _isWaitingVisibility; }
            set {
                _isWaitingVisibility = value;
                OnPropertyChanged();
            }
        }
        
        
        private Visibility _isCheckedVisibility = Visibility.Collapsed;
        public Visibility IsCheckedVisibility
        {
            get { return  _isCheckedVisibility; }
            set {
                _isCheckedVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility _markAsFinishVisibility = Visibility.Collapsed;
        public Visibility MarkAsFinishVisibility
        {
            get { return _markAsFinishVisibility; }
            set
            {
                _markAsFinishVisibility = value;
                OnPropertyChanged();
            }
        }
        //Commands
        private AsyncRelayCommand _takeToUserCommand;
        public AsyncRelayCommand TakeToUserCommand 
        {
            get { return _takeToUserCommand ?? (
                    _takeToUserCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            string? fname = SelectedUser?.Fname;
                            bool res = await DataBaseService.UpdateFieldFromTableById("task", "idUserTaked", $"{SelectedUser?.Id}", _enteredTask.Id);
                            await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "1", _enteredTask.Id);
                            if (res)
                            MessageBox.Show($"Задача успешно взята сотрудником {fname}");
                            MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                        }
                        )
                    ); }
        }
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (
                    _goBackCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                        }
                        )
                    ); }
        }
        private AsyncRelayCommand _markAsFinishCommand;
        public AsyncRelayCommand MarkAsFinishCommand
        {
            get
            {
                return _markAsFinishCommand ?? (
                    _markAsFinishCommand = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            try
                            {
                                /*
                                This is outdated code. block if (_source == "owner"){...}.
                                This point was can runned only from "Пометить как выполнено" on UserPage. Now this button was delete. 
                                 */
                                if (_source == "owner")
                                {
                                    var answer = MessageBox.Show("Задача корректно выполнена?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (answer == MessageBoxResult.Yes)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", _enteredTask.Id);
                                        if (res) MessageBox.Show("Задача выполнена и автоматически проверена");
                                        await DataBaseService.PutFinishedTask(_enteredTask, _enteredUser.Id);
                                        MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                    }
                                    else if (answer == MessageBoxResult.No)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "4", _enteredTask.Id);
                                        if (res)
                                            MessageBox.Show("Задаче присвоен статус \"В ожидании\"", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        else MessageBox.Show("res == false");
                                        MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                    }
                                    else return;
                                }
                                else 
                                {
                                    bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "3", _enteredTask.Id);
                                    await DataBaseService.PutFinishedTask(_enteredTask, _enteredUser.Id);
                                    if (res) MessageBox.Show("Успешно!");
                                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser)); 
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        )
                    );
            }
        }
        private AsyncRelayCommand _takeTaskCommand;
        public AsyncRelayCommand TakeTaskCommand
        {
            get 
            { return _takeTaskCommand ?? (_takeTaskCommand = new AsyncRelayCommand(
                async (obj) =>
                {
                    // recording in db, who taked the task
                    await DataBaseService.UpdateFieldFromTableById(
                        "task",
                        "idUserTaked",
                        EnteredUser.Id.ToString(),
                        EnteredTask.Id
                        );
                    // set status 'В процессе'
                    await DataBaseService.UpdateFieldFromTableById(
                        "task",
                        "idStatus",
                        "1",
                        EnteredTask.Id
                        );
                    MessageBox.Show("Вы успешно взяли задачу!");
                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                }
                )); 
            }
        }
        private AsyncRelayCommand _checkTaskCommand;
        public AsyncRelayCommand CheckTaskCommand
        {
            get
            {
                return _checkTaskCommand ?? (
                        _checkTaskCommand = new AsyncRelayCommand
                        (
                            async (obj) =>
                            {
                                try
                                {
                                    var answer = MessageBox.Show("Задача корректно выполнена?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (answer == MessageBoxResult.Yes)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", _enteredTask.Id);
                                        if (res) MessageBox.Show("Успешно!");
                                    }
                                    else if (answer == MessageBoxResult.No)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "4", _enteredTask.Id);
                                        if (res)
                                            MessageBox.Show("Задаче присвоен статус \"В ожидании\"", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        else MessageBox.Show("res == false");
                                        MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                    }
                                    else return;
                                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        )
                    );
            }
        }
        private async System.Threading.Tasks.Task InitializeAsync()
        {
            if (EnteredTask.IdUserTaked != null && EnteredTask.IdUserTaked != 0)
            {
                IsTaked = Visibility.Visible;
                TakeToUserVisibility = Visibility.Collapsed;

                var users = await DataBaseService.GetUsers();
                var user = users.FirstOrDefault(u => u.Id == EnteredTask.IdUserTaked);
                UserTakedText = user?.Lname ?? "Неизвестный пользователь";
            }
            else
            {
                IsTaked = Visibility.Collapsed;
                TakeToUserVisibility = Visibility.Visible;
                List<User> allUsers = await DataBaseService.GetUsers();
                List<User> filteredUsers = new List<User>();
                foreach (var user in allUsers)
                {
                    foreach (var scope in user.Scopes)
                    {
                        if (scope.Id == EnteredTask.Scope.Id) filteredUsers.Add(user);
                    }
                }
                Users = filteredUsers;
            }
            OnPropertyChanged("UserTakedText");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
