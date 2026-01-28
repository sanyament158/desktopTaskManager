using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class EditUserResponsibilityPageViewModel : INotifyPropertyChanged
    {
        public EditUserResponsibilityPageViewModel(User enteredUser, User selectedUser, List<Category> availScopes) 
        {
            _enteredUser = enteredUser;
            _selectedUser = selectedUser;
            _availScopes = availScopes;
        }
        // Fields & Properties
        private User _enteredUser;
        private string _buttonContent;
        private User _selectedUser;
        public User SelectedUser
        {
            get {  return _selectedUser; }
            set {  _selectedUser = value; 
                OnPropertyChanged(); }
        }
        private List<Category> _availScopes;
        public List<Category> AvailScopes
        {
            get { return _availScopes; }
            set
            {   _availScopes = value;
                OnPropertyChanged();
            }
        }
        private Category _availSelectedScope;
        public Category AvailSelectedScope
        {
            get { return _availSelectedScope; }
            set
            {
                if (_availSelectedScope == value)
                _availSelectedScope = value;
                OnPropertyChanged();
            }
        }
        private Category _userSelectedScope;
        public Category UserSelectedScope
        {
            get { return _userSelectedScope; }
            set
            {
                if (_userSelectedScope == value) 
                _userSelectedScope = value;
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

                        }
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
                            if (res) MessageBox.Show("Успех");
                            AvailScopes.Remove(switchedScope);
                            SelectedUser.Scopes.Add(switchedScope);

                            OnPropertyChanged("AvailScopes");
                            OnPropertyChanged("SelectedUser");
                        }
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
                            MainFrame.mainFrame.GoBack();
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
