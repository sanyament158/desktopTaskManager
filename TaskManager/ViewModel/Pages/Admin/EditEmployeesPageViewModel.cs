using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Annotations;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class EditEmployeesPageViewModel: INotifyPropertyChanged
    {
        public EditEmployeesPageViewModel(User enteredUser) 
        {
            _enteredUser = enteredUser;
            RefreshUsersCommand.Execute(this);
        }
        //Fields & Properties
        private User _enteredUser;
        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                DeleteUserCommand.NotifyCanExecuteChanged();
                GoToUpdateUserCommand.NotifyCanExecuteChanged();
            }
        }
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; } 
            set { _users = value;
                OnPropertyChanged();
            }
        }

        //Commands
        private AsyncRelayCommand _refreshUsersCommand;
        public AsyncRelayCommand RefreshUsersCommand
        {
            get {
                return _refreshUsersCommand ??
                    (_refreshUsersCommand = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            Users = new ObservableCollection<User>(await DataBaseService.GetUsers());
                        }
                        ));
            }
        }
        private AsyncRelayCommand<User> _deleteUserCommand;
        public AsyncRelayCommand<User> DeleteUserCommand
        {
            get
            {
                return _deleteUserCommand ??
                    (_deleteUserCommand = new AsyncRelayCommand<User>(
                        async (obj) =>
                        {
                            try
                            {
                                List<Model.Task> allTasks = await DataBaseService.GetTasks();
                                int userTasks = allTasks.Where(x => x.Owner.Id == SelectedUser.Id).Count();
                                
                                if (userTasks > 0)
                                {
                                    MessageBoxResult answer = MessageBox.Show(
                                            $"{userTasks} задачи автоматически удалятся при уничтожении этого сотрудника",
                                            "Внимание",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning
                                            );
                                    if (answer == MessageBoxResult.Yes)
                                    {
                                        var res = await DataBaseService.DeleteFromTableById("user", SelectedUser.Id);
                                        if (res) MessageBox.Show("Успешно!");
                                        else MessageBox.Show("Ошибка!");
                                    }
                                }
                                else
                                {
                                    var res = await DataBaseService.DeleteFromTableById("user", SelectedUser.Id);
                                    if (res) MessageBox.Show("Успешно!");
                                    else MessageBox.Show("Ошибка!");
                                }
                                RefreshUsersCommand.Execute(this);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        },
                        (obj) => SelectedUser != null && SelectedUser.Username != null
                        ));
            }
        }
        private RelayCommand _goToAddEmployeeCommand;
        public RelayCommand GoToAddEmployeeCommand
        {
            get
            {
                return _goToAddEmployeeCommand ?? (
                    _goToAddEmployeeCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new AddNewUserPage(_enteredUser));
                        }
                        )
                    );
            }
        }
        private AsyncRelayCommand _goToUpdateUser;
        public AsyncRelayCommand GoToUpdateUserCommand
        {
            get { return _goToUpdateUser ?? (
                    _goToUpdateUser = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new UpdateUserPage(_enteredUser, SelectedUser, await DataBaseService.GetPasswordById(SelectedUser.Id)));
                        },
                        () => SelectedUser != null && SelectedUser.Username != null
                        )
                    ); }
        }
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get {
                return _goBackCommand ??
                    (_goBackCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                        }
                        ));
                    }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
