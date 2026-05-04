using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastructure.OfficeDocument;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Infrastucture.OfficeDocument;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;
using TaskManager.View.UserControls;

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

        private List<User> users = new List<User>();
        public string SwitcherButtonContent 
        {
            get
            {
                switch (_tasksControlCounter % 2)
                {
                    case 0:
                        return "Задачи";
                    case 1:
                        return "Сотрудники";
                    default: throw new Exception("out of range (counter)");
                }
            }
        }
        private int _tasksControlCounter = 0;
        public Visibility TasksControlVisibility
        {
            get 
            { 
                switch (_tasksControlCounter % 2)
                    {
                        case 0:
                            return Visibility.Visible;
                        case 1:
                            return Visibility.Collapsed;
                    default: throw new Exception("out of range (counter)");
                    }
                }
        }
        public Visibility UsersControlVisibility
        {
            get 
            { 
                switch (_tasksControlCounter % 2)
                    {
                        case 1:
                            return Visibility.Visible;
                        case 0:
                            return Visibility.Collapsed;
                    default: throw new Exception("out of range (counter)");
                    }
                }
        }

        private WordDocumentReport wordService = new WordDocumentReport();
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
        public DateTime Deadline
        {
            get { return _deadline; }
            set
            {
                _deadline = value;
                OnPropertyChanged();
            }
        }
        // commands

        private RelayCommand _switcherButtonCommand;
        public RelayCommand ControlSwitcherCommand
        {
            get
            {
                return _switcherButtonCommand ?? ( new RelayCommand((obj) =>
                {
                    _tasksControlCounter++;
                    OnPropertyChanged("TasksControlVisibility");
                    OnPropertyChanged("UsersControlVisibility");
                    OnPropertyChanged("SwitcherButtonContent");
                }));
            }
        }
        private AsyncRelayCommand _wordReportCommand;
        public AsyncRelayCommand WordReportCommand
        {
            get { return _wordReportCommand ?? (_wordReportCommand = new AsyncRelayCommand( async (obj)=>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word файлы (*.docx)|*.docx|Все файлы (*.*)|*.*";
                saveFileDialog.DefaultExt = ".docx";
                saveFileDialog.FileName = "отчёт";
                if (saveFileDialog.ShowDialog() == true)
                {
                    if (TasksControlVisibility == Visibility.Visible)
                    {
                        var tasks = await getTaskCollection();
                        using (var documentStream = wordService.CreateWordDocumentFromTasks(tasks))
                        {
                            // Можно сохранить в файл
                            using (var fileStream = File.Create(saveFileDialog.FileName))
                            {
                                documentStream.CopyTo(fileStream);
                            }
                        }
                        MessageBox.Show("Успешно!");
                    }
                    else
                    {
                        var users = await getUserCollection();
                        using (var documentStream = wordService.CreateWordDocumentFromUsers(users))
                        {
                            // Можно сохранить в файл
                            using (var fileStream = File.Create(saveFileDialog.FileName))
                            {
                                documentStream.CopyTo(fileStream);
                            }
                        }
                        MessageBox.Show("Успешно!");
                    }
                }
            })); }
    
        }
        private AsyncRelayCommand _excelReportCommand;
        public AsyncRelayCommand ExcelReportCommand
        {
            get
            {
                return _excelReportCommand ??= new AsyncRelayCommand(async (obj) =>
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel файлы (*.xlsx)|*.xlsx",
                        DefaultExt = ".xlsx",
                        FileName = "отчёт"
                    };

                    if (saveFileDialog.ShowDialog() != true)
                        return;

                    var excelService = new ExcelDocumentReport();

                    if (TasksControlVisibility == Visibility.Visible)
                    {
                        var tasks = await getTaskCollection();

                        var doc = excelService.BuildTasksReport(tasks);

                        doc.SaveAs(saveFileDialog.FileName);
                    }
                    else
                    {
                        var userList = await getUserCollection();
                        var doc = excelService.BuildUsersReport(userList);
                        doc.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Excel отчёт успешно создан!");
                });
            }
        }
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? ( _goBackCommand = new RelayCommand( (obj)=>
            {
                MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
            })); }
        }
        private async System.Threading.Tasks.Task<List<User>> getUserCollection()
        {
            try
            {
                List<User> allUsers = await DataBaseService.GetUsers();
                List<User> filteredUsers = new List<User>();
                foreach (var user in allUsers)
                {
                    foreach (var scope in user.Scopes)
                    {
                        if (scope.Id == SelectedScope.Id) filteredUsers.Add(user);
                    }
                }
                return filteredUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<User>();
            }
        }
        private async System.Threading.Tasks.Task<List<Model.Task>> getTaskCollection()
        {
            try
            {
                List<Model.Task> returnsTask = await DataBaseService.GetTasks();
                returnsTask = returnsTask.Where(t => t.Status.Id == SelectedStatus.Id)
                    .Where(t => t.Scope.Id == SelectedScope.Id)
                    .Where(t => t.Since.Date >= _since.Date && t.Deadline.Date <= _deadline)
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
            users = await DataBaseService.GetUsers();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
