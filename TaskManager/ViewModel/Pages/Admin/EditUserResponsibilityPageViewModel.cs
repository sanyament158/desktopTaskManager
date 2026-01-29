using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class EditUserResponsibilityPageViewModel : INotifyPropertyChanged
    {
        public EditUserResponsibilityPageViewModel(User enteredUser, User selectedUser, List<Category> availScopes, string pass) 
        {
            _enteredUser = enteredUser;
            _selectedUser = selectedUser;
            _availScopes = new ObservableCollection<Category>(availScopes);
            _userScopes = new ObservableCollection<Category>(selectedUser.Scopes);

            _pass = pass;
        }
        // Fields & Properties
        private readonly string _pass = ""; // only for new UpdateUserPage(pass) instead GoBack();
        
        private User _enteredUser;
        private string _buttonContent;
        private User _selectedUser;
        public User SelectedUser
        {
            get {  return _selectedUser; }
            set {  _selectedUser = value; 
                OnPropertyChanged(); }
        }
        private ObservableCollection<Category> _availScopes;
        public ObservableCollection<Category> AvailScopes
        {
            get { return _availScopes; }
            set
            {   _availScopes = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Category> _userScopes;
        public ObservableCollection<Category> UserScopes
        {
            get { return _userScopes; }
            set
            {
                _userScopes = value;
                OnPropertyChanged();
            }
        }
        private Category _availSelectedScope;
        public Category AvailSelectedScope
        {
            get {
                return _availSelectedScope; }
            set
            {
                _availSelectedScope = value;
                _moveToUserCommand.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private Category _userSelectedScope;
        public Category UserSelectedScope
        {
            get { return _userSelectedScope; }
            set
            {
                _userSelectedScope = value;
                _moveToAvailCommand.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        // Commands
        private AsyncRelayCommand _moveToAvailCommand;
        public AsyncRelayCommand MoveToAvailCommand
        {
            get { return _moveToAvailCommand ?? (
                    _moveToAvailCommand = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            var switchedScope = UserScopes.Where(x => x.Id == UserSelectedScope.Id).First();
                            await DataBaseService.RemoveResponsibility(SelectedUser.Id, switchedScope.Id);

                            AvailScopes.Add(switchedScope);
                            UserScopes.Remove(switchedScope);
                            SelectedUser.Scopes.Remove(switchedScope);
                        },
                        () => UserSelectedScope != null && UserSelectedScope.Id != 0
                        )
                    ); }
        }
        private AsyncRelayCommand _moveToUserCommand;
        public AsyncRelayCommand MoveToUserCommand
        {
            get
            {
                return _moveToUserCommand ?? (
                    _moveToUserCommand = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            var switchedScope = AvailScopes.Where(x => x.Id == AvailSelectedScope.Id).First();
                            var res = await DataBaseService.PutResponsibility(AvailSelectedScope.Id, SelectedUser.Id);

                            AvailScopes.Remove(switchedScope);
                            UserScopes.Add(switchedScope);
                            SelectedUser.Scopes.Add(switchedScope);
                        },
                        () => AvailSelectedScope != null && AvailSelectedScope.Id != 0
                        )
                    );
            }
        }
        private AsyncRelayCommand _refreshAvailScopes;
        public AsyncRelayCommand RefreshAvailScopes
        {
            get { return _refreshAvailScopes ?? (
                    new AsyncRelayCommand(
                        async ()=>
                        {
                            List<Category> allScopes = await DataBaseService.GetCategories();
                            List<Category> availScopes = allScopes
                            .Where(s => !SelectedUser.Scopes
                            .Any(sc => sc.Id == s.Id))
                            .ToList();
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
                            MainFrame.mainFrame.Navigate(new UpdateUserPage(_enteredUser, SelectedUser, _pass));
                        }
                        )
                    ); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // internal methods
    }
}
