using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
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
