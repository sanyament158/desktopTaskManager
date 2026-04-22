using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    /// <summary>
    /// this class should calling a report service. 
    /// params for reports: 
    ///    (4 params) date, owner, status, scope.
    ///  users can: 
    ///     get report taked tasks. all tasks having user's scope. without the finished status.
    ///  admins can: 
    ///     to filter all props and create a report. and get the all finishedTasks.
    ///     
    /// scheme: 
    ///    command -> call the method(model objects)
    ///             <- get document object
    ///    saving .docx document 
    /// </summary>
    public class ReportsPageViewModel: INotifyPropertyChanged
    {
        public ReportsPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            _ = InitializeAsync();
            _since = DateTime.Now;
            _deadline = DateTime.Now + TimeSpan.FromDays(1);
        }
        private User _enteredUser;
        private List<Category> _scopes;
        public List<Category> Scopes
        {
            set
            {
                _scopes = value;
                OnPropertyChanged();
            }
            get
            {
                return _scopes;
            }
        }
        private List<Status> _statuses;
        public List<Status> Statuses
        {
            set
            {
                _statuses = value;
                OnPropertyChanged();
            }
            get
            {
                return _statuses;
            }
        }
        private Status _selectedStatus;
        public Status SelectedStatus
        {
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
            get
            {
                return _selectedStatus;
            }
        }
        private Category _selectedScope;
        public Category SelectedScope
        {
            set
            {
                _selectedScope = value;
                OnPropertyChanged();
            }
            get
            {
                return _selectedScope;
            }
        }
        private DateTime _since;
        public DateTime Since
        {
            get { return _since; }
            set
            {
                _since = value;
                OnPropertyChanged();
            }
        }
        private DateTime _deadline;
        public string Deadline
        {
            get { return _deadline.ToString("yyyy-MM-dd"); }
            set
            {
                _deadline = DateTime.ParseExact(
                         value,
                         "yyyy-MM-dd",
                         System.Globalization.CultureInfo.InvariantCulture);
                OnPropertyChanged();
            }
        }
        // commands
        private AsyncRelayCommand _wordReportCommand;
        public AsyncRelayCommand WordReportCommand
        {
            get { return _wordReportCommand ?? (_wordReportCommand = new AsyncRelayCommand( async (obj)=>
            {
                var tasks = await getTaskCollection();
                MessageBox.Show(tasks.Count().ToString());
            })); }
        }
        private async System.Threading.Tasks.Task<List<Model.Task>> getTaskCollection()
        {
            try
            {
                List<Model.Task> returnsTask = await DataBaseService.GetTasks();
                returnsTask = returnsTask.Where(t => t.Status.Id == SelectedStatus.Id)
                    .Where(t => t.Scope.Id == SelectedScope.Id)
                    .Where(t => t.Since.Date >= _since.Date //&& t.Deadline.Date <= _deadline.Date
                                                            )
                    .ToList();
                return returnsTask;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<Model.Task>();
            }
        }
        private async System.Threading.Tasks.Task InitializeAsync()
        {
            Scopes = await DataBaseService.GetCategories();
            Statuses = await DataBaseService.GetStatuses();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
