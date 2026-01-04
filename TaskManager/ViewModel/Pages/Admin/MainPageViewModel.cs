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
using TaskManager.View.Pages.Users;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            RefreshCategoriesCommand.Execute(this);
            RefreshTasksCommand.Execute(this);
        }
        // fields & properties
        private Model.User _enteredUser;
        private ObservableCollection<Model.Task> _tasks;
        public ObservableCollection<Model.Task> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }
        private Model.Task _selectedTask;
        public Model.Task SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                DeleteTaskCommand.NotifyCanExecuteChanged();
                OnPropertyChanged();
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
                            List<Model.Task> allTasks = await DataBaseService.GetTasks();
                            Tasks = new ObservableCollection<Model.Task>(allTasks.Where(obj => obj.Status.Id == 1));
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
                            () => (SelectedTask != null && SelectedTask.Id != 0)
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
                        (obj) => _enteredUser.Role.Id == 1
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
                            MainFrame.mainFrame.Navigate(new UpdateTaskPage(_enteredUser, _selectedTask, await DataBaseService.GetCategories()));
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
                        (obj) => _enteredUser.Role.Id == 1
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
        //employees commands
        public AsyncRelayCommand MarkAsFinishCommand
        {
            get { return _markAsFinishCommand ?? (
                    _markAsFinishCommand = new AsyncRelayCommand(
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
                                    MainFrame.mainFrame.Navigate(new ShowTaskDetailsPage(_enteredUser, task));
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
                ObservableCollection<Model.Task> allTasks = new ObservableCollection<Model.Task>(await DataBaseService.GetTasks());
                Tasks = new ObservableCollection<Model.Task>(allTasks.Where(obj => obj.Scope.Id == _selectedCategory.Id && obj.Status.Id == 1));
            }
        }

    }
}
