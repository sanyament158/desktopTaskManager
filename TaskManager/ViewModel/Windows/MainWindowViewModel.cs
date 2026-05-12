using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using System.Windows.Media;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Model;
using TaskManager.View.Pages;
using TaskManager.View.Pages.Admin;
using TaskManager.View.Windows;

namespace TaskManager.ViewModel.Windows
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public MainWindowViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
            OnPageChanged();
        }
        //fields and props
        User _enteredUser;

        

        private enum Pages 
        {
            Home, Scopes, Employees, AddTask, ViewResponsibilities, Reports
        }
        //commands
        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand 
        {
            get { return _exitCommand ?? (
                    _exitCommand = new RelayCommand(
                        (obj) =>
                        {
                            var loginWindow = new LoginWindow();
                            MainFrame.loginWindow.Show();
                            MainFrame.mainWindow.Close();
                        }
                        ) 
                    ); }
        }
        private RelayCommand _viewResponsibilitiesCommand;
        public RelayCommand ViewResponsibilitiesCommand 
        {
            get { return _viewResponsibilitiesCommand ?? (
                    _viewResponsibilitiesCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new ViewResponsibilities(_enteredUser));
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }
        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand 
        {
            get { return _homeCommand ?? (
                    _homeCommand = new RelayCommand(
                        (obj) =>
                        {
                            OnPageChanged();
                            MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                        }
                        ) 
                    ); }
        }
        private RelayCommand _scopesCommand;
        public RelayCommand ScopesCommand
        {
            get { return _scopesCommand ?? (
                    _scopesCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new EditScopesPage(_enteredUser));
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }

        private RelayCommand _employeesCommand;
        public RelayCommand EmployeesCommand
        {
            get { return _employeesCommand ?? (
                    _employeesCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new EditEmployeesPage(_enteredUser));
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }

        private RelayCommand _addTaskCommand;
        public RelayCommand AddTaskCommand
        {
            get { return _addTaskCommand ?? (
                    _addTaskCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new AddTaskPage(_enteredUser));
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }
        private RelayCommand _loggingCommand;
        public RelayCommand LoggingCommand
        {
            get { return _loggingCommand ?? (
                    _loggingCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new LoggingPage(_enteredUser));
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }

        private RelayCommand _reportsCommand;
        public RelayCommand ReportsCommand 
        {
            get { return _reportsCommand ?? (
                    _reportsCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new ReportsPage(_enteredUser)); 
                            OnPageChanged();
                        }
                        ) 
                    ); }
        }

        private SolidColorBrush normal = (SolidColorBrush)new BrushConverter().ConvertFromString("#432818");
        private SolidColorBrush pressed = (SolidColorBrush)new BrushConverter().ConvertFromString("#2E1A10");
        public SolidColorBrush ScopesColor {  get; set; }
        public SolidColorBrush EmployeesColor { get; set; }
        public SolidColorBrush AddTaskColor { get; set; }
        public SolidColorBrush ViewResponsibilitiesColor { get; set; }
        public SolidColorBrush ReportsColor { get; set; }
        public SolidColorBrush LoggingColor { get; set; }
        private void OnPageChanged([CallerMemberName]string src="")
        {
            ScopesColor = normal;
            EmployeesColor = normal;
            AddTaskColor = normal;
            ViewResponsibilitiesColor = normal;
            ReportsColor = normal;
            LoggingColor = normal;

            switch (src)
            {
                case "ScopesCommand":
                    ScopesColor = pressed;
                    break;
                case "EmployeesCommand":
                    EmployeesColor = pressed;
                    break;
                case "AddTaskCommand":
                    AddTaskColor = pressed;
                    break;
                case "ViewResponsibilitiesCommand":
                    ViewResponsibilitiesColor = pressed;
                    break;
                case "ReportsCommand":
                    ReportsColor = pressed;
                    break;
                case "LoggingCommand":
                    LoggingColor = pressed;
                    break;
                default:
                    break;
            }
            OnPropertyChanged("ScopesColor");
            OnPropertyChanged("EmployeesColor");
            OnPropertyChanged("AddTaskColor");
            OnPropertyChanged("ViewResponsibilitiesColor");
            OnPropertyChanged("ReportsColor");
            OnPropertyChanged("LoggingColor");
            /*
             public SolidColorBrush ScopesColor {  get; set; }
                    public SolidColorBrush EmployeesColor { get; set; }
                    public SolidColorBrush AddTaskColor { get; set; }
                    public SolidColorBrush ViewResponsibilitiesColor { get; set; }
                    public SolidColorBrush ReportsColor { get; set; } 
             */
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
