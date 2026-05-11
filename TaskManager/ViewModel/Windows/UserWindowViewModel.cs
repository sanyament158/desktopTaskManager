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
using TaskManager.View.Pages.Admin;
using TaskManager.View.Pages.Users;
using TaskManager.View.Windows;

namespace TaskManager.ViewModel.Windows
{
    public class UserWindowViewModel: INotifyPropertyChanged
    {
        public UserWindowViewModel(User enteredUser) 
        {
            _enteredUser = enteredUser;
            OnPageChanged();
        }

        //fields & properties 
        private User _enteredUser;

        // commands
        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand
        {
            get {
                return _homeCommand ?? (
                    _homeCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                            OnPageChanged();
                        }
                        )
                    ); }
        }
        private RelayCommand _takedTasksCommand;
        public RelayCommand TakedTasksCommand 
        {
            get {
                return _takedTasksCommand ?? (
                    _takedTasksCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new TakedTasksPage(_enteredUser));
                            OnPageChanged();
                        }
                        )
                    ); }
        }
        private RelayCommand _myTasksCommand;
        public RelayCommand MyTasksCommand 
        {
            get {
                return _myTasksCommand ?? (
                    _myTasksCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new MyTasksPage(_enteredUser));
                            OnPageChanged();
                        }
                        )
                    ); }
        }
        private RelayCommand _finishedTasksCommand;
        public RelayCommand FinishedTasksCommand 
        {
            get {
                return _finishedTasksCommand ?? (
                    _finishedTasksCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new FinishedTasksPage(_enteredUser));
                            OnPageChanged();
                        }
                        )
                    ); }
        }
        private RelayCommand _addTaskCommand;
        public RelayCommand AddTaskCommand 
        {
            get {
                return _addTaskCommand ?? (
                    _addTaskCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new AddTaskPage(_enteredUser));
                            OnPageChanged();
                        }
                        )
                    ); }
        }
        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand 
        {
            get {
                return _exitCommand ?? (
                    _exitCommand = new RelayCommand(
                        (obj) =>
                        {
                            var window = new LoginWindow();
                            window.Show();
                            MainFrame.userWindow.Close(); 
                        }
                        )
                    ); }
        }


        private SolidColorBrush normal = (SolidColorBrush)new BrushConverter().ConvertFromString("#432818");
        private SolidColorBrush pressed = (SolidColorBrush)new BrushConverter().ConvertFromString("#2E1A10");
        public SolidColorBrush TakedTasksColor { get; set; }
        public SolidColorBrush MyTasksColor { get; set; }
        public SolidColorBrush FinishedTasksColor { get; set; }
        public SolidColorBrush AddTaskColor { get; set; }
        private void OnPageChanged([CallerMemberName] string src = "")
        {
            TakedTasksColor = normal;
            MyTasksColor = normal;
            FinishedTasksColor = normal;
            AddTaskColor = normal;

            switch (src)
            {
                case "TakedTasksCommand":
                    TakedTasksColor = pressed;
                    break;
                case "MyTasksCommand":
                    MyTasksColor = pressed;
                    break;
                case "FinishedTasksCommand":
                    FinishedTasksColor = pressed;
                    break;
                case "AddTaskCommand":
                    AddTaskColor = pressed;
                    break;
                default:
                    break;
            }
            OnPropertyChanged("TakedTasksColor");
            OnPropertyChanged("MyTasksColor");
            OnPropertyChanged("FinishedTasksColor");
            OnPropertyChanged("AddTaskColor");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
