using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastructure.OfficeDocument;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Infrastucture.OfficeDocument;
using TaskManager.Model;
using TaskManager.View.Pages.Users;

namespace TaskManager.ViewModel.Pages.Users
{
    public class TakedTasksPageViewModel: INotifyPropertyChanged
    {

        public TakedTasksPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            _ = InitializeAsync();
        }

        // fields && properties
        private User _enteredUser;
        private List<Model.Task> _tasks = new List<Model.Task>();
        public List<Model.Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value;
                OnPropertyChanged();
            }
        }
        private Model.Task _selectedTask;
        public Model.Task SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // commands
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand((obj) =>
            {
                MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));

            } ) ); }
        }
        private AsyncRelayCommand _reportCommand;
        public AsyncRelayCommand ReportCommand
        {
            get { return _reportCommand ?? (_reportCommand = new AsyncRelayCommand(
                async () =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                    saveFileDialog.DefaultExt = ".xlsx";
                    saveFileDialog.FileName = "отчёт_по_задачам";

                    //if (saveFileDialog.ShowDialog() == true)
                    //{
                    //    ExcelDocumentReport excelService = new ExcelDocumentReport();
                    //    var users = await DataBaseService.GetUsers();
                    //    excelService.UsersDict = users;
                    //    var report = excelService.ExportTasks(Tasks);
                        
                    //    report.SaveAs(saveFileDialog.FileName);
                    //    MessageBox.Show("success");
                    //}
                }
                )); }
        }
        private RelayCommand _goToShowDetailsCommand;
        public RelayCommand GoToShowDetailsCommand
        {
            get
            {
                return _goToShowDetailsCommand ?? (
                    _goToShowDetailsCommand = new RelayCommand(
                            (obj) =>
                            {
                                if (obj is Model.Task task)
                                {
                                    if (_enteredUser.Id == task.Owner.Id) MainFrame.mainFrame.Navigate(new ShowTaskDetailsPage(_enteredUser, task, "owner"));
                                    else MainFrame.mainFrame.Navigate(new ShowTaskDetailsPage(_enteredUser, task, "user"));
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка!");
                                }
                            },
                            (obj) => (SelectedTask != null && SelectedTask.Id != 0)
                        )
                    );
            }
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            List<Model.Task> allTasks = await DataBaseService.GetTasks();
            List<Model.Task> filteredTasks = allTasks.Where(t => t.IdUserTaked == _enteredUser.Id).Where(t => t.Status.Id != 2).ToList();
            Tasks = filteredTasks;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
