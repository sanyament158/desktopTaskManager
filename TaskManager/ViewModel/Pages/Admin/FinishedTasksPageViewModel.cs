using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Infrastucture.Navigation;
using TaskManager.View.Pages;
using TaskManager.Model;
using System.Collections.ObjectModel;
using System.Windows;
using TaskManager.Infrastucture.Network;
using CommunityToolkit.Mvvm.Input;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class FinishedTasksPageViewModel : INotifyPropertyChanged
    {
        public FinishedTasksPageViewModel(User _enteredUser, List<Model.Task> tasks, List<Category> scopes)
        {
            this._enteredUser = _enteredUser;
            _tasks = new ObservableCollection<Model.Task>(tasks.Where(x => x.Status.Id == 2));
            _scopes = new ObservableCollection <Category> (scopes);
            _since = DateTime.Now;
            _deadline = DateTime.Now + TimeSpan.FromDays(1);
        }
        // Fields & Properties 
        private readonly User _enteredUser;

        private Category _selectedScope;
        public Category SelectedScope
        {
            get { return _selectedScope; }
            set
            {
                _selectedScope = value; 
                RefreshTasks.Execute(this);
                OnPropertyChanged();
            }
        }
        private DateTime _since;
        public DateTime Since
        {
            get { return _since; }
            set { _since = value;
                RefreshTasks.Execute(this);
                OnPropertyChanged(); }
        }
        private DateTime _deadline;
        public DateTime Deadline
        {
            get { return _deadline; }
            set { _deadline = value;
                RefreshTasks.Execute(this);
                OnPropertyChanged(); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value;
                RefreshTasks.Execute(this);
                OnPropertyChanged(); }
        }
        private ObservableCollection<Model.Task> _tasks;
        public ObservableCollection<Model.Task> Tasks
        {
            get { return _tasks; }
            set { _tasks =  value; OnPropertyChanged(); }
        }
        private ObservableCollection<Category> _scopes;
        public ObservableCollection<Category> Scopes
        {
            get { return _scopes; }
            set { _scopes = value;
                RefreshTasks.Execute(this);
                OnPropertyChanged(); }
        }
        // Commands
        private AsyncRelayCommand _refreshTasks;
        public AsyncRelayCommand RefreshTasks
        {
            get { return _refreshTasks ?? (
                    _refreshTasks = new AsyncRelayCommand(
                            async (obj) =>
                            {
                                List<Model.Task> filteredTasks = await DataBaseService.GetTasks();
                                filteredTasks = filteredTasks.Where(x => x.Since.Date >= Since.Date && x.Deadline.Date <= Deadline.Date)
                                    .Where(x => x.Status.Id == 2)
                                    .ToList();
                                if (SelectedScope != null) filteredTasks = filteredTasks.Where(x => x.Scope.Id == SelectedScope.Id).ToList();
                                if (!string.IsNullOrWhiteSpace(Title)) filteredTasks = filteredTasks.Where(x => x.Title.Contains(Title)).ToList();
                                Tasks = new ObservableCollection<Model.Task>(filteredTasks);
                            }
                        )
                    ); }
        }
        private AsyncRelayCommand _cancelSelection;
        public AsyncRelayCommand CancelSelection
        {
            get { return _cancelSelection ?? (
                    _cancelSelection = new AsyncRelayCommand(
                        async () =>
                        {
                            SelectedScope = null;
                            RefreshTasks.Execute(this);
                        }
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
                            MainFrame.mainFrame.GoBack();
                        }
                        )
                    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
