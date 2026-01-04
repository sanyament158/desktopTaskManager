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
        public ShowTaskDetailsPageViewModel(User enteredUser, Model.Task enteredTask)
        {
            _enteredTask = enteredTask;
            _enteredUser = enteredUser;
        }

        //Fields & Properties
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
                                bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", _enteredTask.Id);
                                if (res) MessageBox.Show("Успешно!");
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
