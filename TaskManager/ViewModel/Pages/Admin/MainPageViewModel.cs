using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Ribbon.Primitives;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel(Model.User enteredUser)
        {
            _enteredUser = enteredUser;
            RefreshCategoriesCommand.Execute(this);
            RefreshTasksCommand.Execute(this);
        }
        // fields & properties
        private Model.User _enteredUser;
        private TaskHumanReadable _selectedTask;
        public TaskHumanReadable SelectedTask
        {
            get {  return _selectedTask; }
            set { 
                _selectedTask = value;
                OnPropertyChanged();
                DeleteTaskCommand.NotifyCanExecuteChanged();
            }
        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                SortCategoryList();

            }
        }
        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
            get
            {
                return _categories;
            }
        }
        private ObservableCollection<TaskHumanReadable> _tasks;
        public ObservableCollection<TaskHumanReadable> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }

        // commands
        private AsyncRelayCommand _refreshCategoriesCommand;
        public IAsyncRelayCommand RefreshCategoriesCommand
        {
            get {
                return _refreshCategoriesCommand ?? 
                    (_refreshCategoriesCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            Categories = new ObservableCollection<Category>(await DataBaseService.GetCategories());
                        }
                        )
                    );
                }
        }
        private AsyncRelayCommand _refreshTasksCommand;
        public AsyncRelayCommand RefreshTasksCommand
        {
            get
            {
                return _refreshTasksCommand ?? (
                    _refreshTasksCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            List<TaskHumanReadable> allTasks = await DataBaseService.GetTasksHumanReadable();
                            Tasks = new ObservableCollection<TaskHumanReadable>(allTasks.Where(obj => obj.Status == "В процессе"));
                        }
                        )
                    );
            }
        }
        private RelayCommand _cancelSelectionCommand;
        public RelayCommand CancelSelectionCommand
        {
            get {
                return _cancelSelectionCommand ??(
                    _cancelSelectionCommand = new RelayCommand(
                        (obj) =>
                        {
                            SelectedCategory = null;
                            RefreshTasksCommand.Execute(this);
                        },
                        (obj) => SelectedCategory != null
                        )
                    );
            }
        }
        private AsyncRelayCommand _deleteTaskCommand;
        public AsyncRelayCommand DeleteTaskCommand
        {
            get
            {
                return _deleteTaskCommand ??
                    (
                        _deleteTaskCommand = new AsyncRelayCommand(
                            async (obj) =>
                            {
                                try
                                {
                                    int taskId = SelectedTask.Id;
                                    var res = await DataBaseService.DeleteFromTableById("task", taskId);
                                    if (res) MessageBox.Show("Успешно!");
                                    else MessageBox.Show("Ошибка!");
                                    RefreshTasksCommand.Execute(this);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            },
                            () => SelectedTask != null && SelectedTask.Title != null
                            )
                    );
            }
        }
        private RelayCommand _goToEditScopesCommand;
        public RelayCommand GoToEditScopesCommand
        {
            get {
                return _goToEditScopesCommand ??
                    (_goToEditScopesCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(
                                new EditScopesPage(_enteredUser)
                                );
                        },
                        (obj) => _enteredUser.IdRole == 1
                        ));
            }
        }
        private AsyncRelayCommand _goToUpdateTaskCommand;
        public AsyncRelayCommand GoToUpdateTaskCommand
        {
            get { return _goToUpdateTaskCommand ?? (
                    _goToUpdateTaskCommand = new AsyncRelayCommand(
                        async (obj) => 
                        {
                            List<Model.Task> allTasks = await DataBaseService.GetTasks();
                            Model.Task task = allTasks.Where((obj) => obj.Id == SelectedTask.Id).First();
                            MainFrame.mainFrame.Navigate(new UpdateTaskPage(_enteredUser, SelectedTask, task));
                        }
                        )
                    ); }
        }
        private RelayCommand _goToEditEmployeesCommand;
        public RelayCommand GoToEditEmployeesCommand
        {
            get
            {
                return _goToEditEmployeesCommand ??
                    (_goToEditEmployeesCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(
                                new EditEmployeesPage(_enteredUser) 
                                );
                        },
                        (obj) => _enteredUser.IdRole == 1
                        ));
            }
        }
        private RelayCommand _goToAddTaskCommand;
        public RelayCommand GoToAddTaskCommand
        {
            get { return _goToAddTaskCommand ??
                    (
                    _goToAddTaskCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new AddTaskPage(_enteredUser));
                        }
                        )
                    ); }
        }
        private AsyncRelayCommand _markAsFinishCommand;
        public AsyncRelayCommand MarkAsFinishCommand
        {
            get { return _markAsFinishCommand ?? (
                    new AsyncRelayCommand(
                        async (obj) =>
                        {
                            try
                            {
                                bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", SelectedTask.Id);
                                if (res) MessageBox.Show("Успешно!");
                                RefreshTasksCommand.Execute(this);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        )
                    ); }
        }
        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (
                    _exitCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new LoginPage());
                        }
                        )
                    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        //methods
        private async void SortCategoryList()
        {

            if (_selectedCategory != null)
            {
                ObservableCollection<TaskHumanReadable> allTasks = new ObservableCollection<TaskHumanReadable>(await DataBaseService.GetTasksHumanReadable());
                Tasks = new ObservableCollection<TaskHumanReadable>(allTasks.Where(obj => obj.Scope == _selectedCategory.Name && obj.Status == "В процессе"));
            }
        }

    }
}
