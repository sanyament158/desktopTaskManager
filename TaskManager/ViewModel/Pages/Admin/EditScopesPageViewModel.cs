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

namespace TaskManager.ViewModel.Pages.Admin
{
    public class EditScopesPageViewModel : INotifyPropertyChanged
    {
        public EditScopesPageViewModel()
        {
            RefreshScopesCommand.Execute(this);
        }
        //Fields & properties
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
        public IAsyncRelayCommand RefreshScopesCommand
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
                            MainFrame.mainFrame.Navigate(new AddNewScopePage());
                        }
                        )
                    ); }
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
