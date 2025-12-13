using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Model;
using TaskManager.Infrastucture.Network;
using System.Collections.ObjectModel;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TaskManager.Infrastucture.Navigation;
using TaskManager.View.Pages.Admin;

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
        private User _enteredUser;
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
        public IAsyncRelayCommand RefreshTasksCommand
        {
            get
            {
                return _refreshTasksCommand ?? (
                    _refreshTasksCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            Tasks = new ObservableCollection<TaskHumanReadable>(await DataBaseService.GetTasks());
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
                ObservableCollection<TaskHumanReadable> allTasks = new ObservableCollection<TaskHumanReadable>(await DataBaseService.GetTasks());
                Tasks = new ObservableCollection<TaskHumanReadable>(allTasks.Where(obj => obj.Scope == _selectedCategory.Name));
            }
        }

    }
}
