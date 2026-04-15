using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Users;

namespace TaskManager.ViewModel.Pages.Users
{
    public class ShowTaskDetailsPageViewModel: INotifyPropertyChanged
    {
        public ShowTaskDetailsPageViewModel(User enteredUser, Model.Task enteredTask, string source)
        {
            MessageBox.Show(source);


            /*
            finishedUser entity very importance. with her, user can looking 
            tasks, which he was finished or finished && checked.
             */
            /*
            may be: 
                user, 
                taked,
                finished,
                owner
             */
            switch (source)
            {
                case "owner":
                    {
                        switch (enteredTask.Status.Id)
                        {
                            case 1:
                                MarkAsFinishVisibility = Visibility.Visible;
                                break;
                            case 3:
                                IsCheckedVisibility = Visibility.Visible;
                                break;
                        }
                        break;
                    }
                case "user":
                    {
                        switch (enteredTask.Status.Id)
                        {
                            case 1:
                                MarkAsFinishVisibility = Visibility.Visible;
                                break;
                            case 4:
                                IsWaitingVisibility = Visibility.Visible;
                                break;
                        }
                        break;
                    }
            } 

            _source = source;
            _enteredTask = enteredTask;
            _enteredUser = enteredUser;
        }

        //Fields & Properties
        private readonly string _source;
        private User _enteredUser;
        public User EnteredUser
        {
            get { return _enteredUser; }
            set { _enteredUser = value;
                OnPropertyChanged();
            }
        }
        private Model.Task _enteredTask;
        public Model.Task EnteredTask
        {
            get { return  _enteredTask; }
            set { _enteredTask = value;
                OnPropertyChanged();
            }
        }

            //buttons visibility
        private Visibility _isWaitingVisibility = Visibility.Collapsed;
        public Visibility IsWaitingVisibility
        {
            get { return  _isWaitingVisibility; }
            set {
                _isWaitingVisibility = value;
                OnPropertyChanged();
            }
        }
        
        
        private Visibility _isCheckedVisibility = Visibility.Collapsed;
        public Visibility IsCheckedVisibility
        {
            get { return  _isCheckedVisibility; }
            set {
                _isCheckedVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility _markAsFinishVisibility = Visibility.Collapsed;
        public Visibility MarkAsFinishVisibility
        {
            get { return _markAsFinishVisibility; }
            set
            {
                _markAsFinishVisibility = value;
                OnPropertyChanged();
            }
        }
        //Commands
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (
                    _goBackCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                        }
                        )
                    ); }
        }
        private AsyncRelayCommand _markAsFinishCommand;
        public AsyncRelayCommand MarkAsFinishCommand
        {
            get
            {
                return _markAsFinishCommand ?? (
                    _markAsFinishCommand = new AsyncRelayCommand(
                        async (obj) =>
                        {
                            try
                            {
                                if (_source == "owner")
                                {
                                    var answer = MessageBox.Show("Подтвердите проверку...", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                                    if (answer == MessageBoxResult.OK)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", _enteredTask.Id);
                                        if (res) MessageBox.Show("Задача выполнена и автоматически проверена");
                                        await DataBaseService.PutFinishedTask(_enteredTask, _enteredUser.Id);
                                        MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                    }
                                }
                                else 
                                {
                                    bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "3", _enteredTask.Id);
                                    await DataBaseService.PutFinishedTask(_enteredTask, _enteredUser.Id);
                                    if (res) MessageBox.Show("Успешно!");
                                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser)); 
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        )
                    );
            }
        }
        private AsyncRelayCommand _takeTaskCommand;
        public AsyncRelayCommand TakeTaskCommand
        {
            get 
            { return _takeTaskCommand ?? (_takeTaskCommand = new AsyncRelayCommand(
                async (obj) =>
                {
                    // update field 'idUserTaked' to enteredUser.id; status = taked;
                    
                    // recording in db, who taked the task
                    await DataBaseService.UpdateFieldFromTableById(
                        "task",
                        "idUserTaked",
                        EnteredUser.Id.ToString(),
                        EnteredTask.Id
                        );
                    // set status 'В процессе'
                    await DataBaseService.UpdateFieldFromTableById(
                        "task",
                        "idStatus",
                        "1",
                        EnteredTask.Id
                        );
                    MessageBox.Show("Вы успешно взяли задачу!");
                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                }
                )); 
            }
        }
        private AsyncRelayCommand _checkTaskCommand;
        public AsyncRelayCommand CheckTaskCommand
        {
            get
            {
                return _checkTaskCommand ?? (
                        _checkTaskCommand = new AsyncRelayCommand
                        (
                            async (obj) =>
                            {
                                try
                                {
                                    var answer = MessageBox.Show("Подтвердите проверку...", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                                    if (answer == MessageBoxResult.OK)
                                    {
                                        bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", _enteredTask.Id);
                                        if (res) MessageBox.Show("Успешно!");
                                    }
                                    else return;
                                    MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                }
                            catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        )
                    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
