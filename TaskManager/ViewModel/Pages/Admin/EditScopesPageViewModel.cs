using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Model;
using TaskManager.Infrastucture.Network;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TaskManager.Infrastucture.Navigation;
using TaskManager.View.Pages.Admin;
using System.Windows;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class EditScopesPageViewModel : INotifyPropertyChanged
    {
        public EditScopesPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            RefreshScopesCommand.Execute(this);
        }
        //Fields & properties
        private User _enteredUser;
        private Category _selectedScope;
        public Category SelectedScope
        {
            get { return _selectedScope; }
            set
            {
                _selectedScope = value;
                OnPropertyChanged();
                DeleteScopeCommand.NotifyCanExecuteChanged();
            }
        }
        private ObservableCollection<Category> _scopes;
        public ObservableCollection<Category> Scopes
        {
            get { return _scopes; }
            set { 
                _scopes = value;
                OnPropertyChanged();
            }
        }

        //Commands
        private AsyncRelayCommand _refreshScopesCommand;
        public AsyncRelayCommand RefreshScopesCommand
        {
            get {
                return _refreshScopesCommand ?? (
                    _refreshScopesCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            Scopes = new ObservableCollection<Category>(await DataBaseService.GetCategories());
                        }
                        )
                    ); }
        }
        private RelayCommand _goToAddScopeCommand;
        public RelayCommand GoToAddScopeCommand
        {
            get { return _goToAddScopeCommand ?? (
                    _goToAddScopeCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new AddNewScopePage(_enteredUser));
                        }
                        )
                    ); }
        }
        private RelayCommand _goToUpdateScopeCommand;
        public RelayCommand GoToUpdateScopeCommand
        {
            get
            {
                return _goToUpdateScopeCommand ?? (
                    _goToUpdateScopeCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new UpdateScopePage(_enteredUser, SelectedScope));
                        },
                        (obj) => SelectedScope != null && SelectedScope.Name != null
                        )
                    );
            }
        }
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (
                    _goBackCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                        }
                        )
                    );
            }
        }
        private AsyncRelayCommand _deleteScopeCommand;
        public AsyncRelayCommand DeleteScopeCommand
        {
            get { return _deleteScopeCommand ??
                    (
                        _deleteScopeCommand = new AsyncRelayCommand(
                            async (obj) =>
                            {
                                try
                                {
                                    int categoryId = SelectedScope.Id;
                                    var res = await DataBaseService.DeleteFromTableById("scope", categoryId);
                                    if (res) MessageBox.Show("Успешно!");
                                    else MessageBox.Show("Ошибка!");
                                    RefreshScopesCommand.Execute(this);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            },
                            () => SelectedScope!= null && SelectedScope.Name!= null
                            )                        
                    );
                    }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
