using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewUserPageViewModel: INotifyPropertyChanged
    {
        public AddNewUserPageViewModel(User enteredUser)
        {
            _enteredUser = enteredUser;
        }
        private User _enteredUser;

        private string _inputedLogin;
        public string InputedLogin
        {
            get { return _inputedLogin; }
            set { _inputedLogin = value;
                OnPropertyChanged();
            }
        }
        private string _inputedLname;
        public string InputedLname
        {
            get { return _inputedLname; }
            set
            {
                _inputedLname = value;
                OnPropertyChanged();
            }
        }
        private int _inputedIdRole;
        public int InputedIdRole
        {
            get { return _inputedIdRole; }
            set
            {
                _inputedIdRole = value;
                OnPropertyChanged();
            }
        }
        private string _inputedPassword;
        public string InputedPassword
        {
            get { return _inputedPassword; }
            set
            {
                _inputedPassword = value;
                OnPropertyChanged();
            }
        }


        //Commands 
        private AsyncRelayCommand<Button> _acceptCommand;
        public AsyncRelayCommand<Button> AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (
                    _acceptCommand = new AsyncRelayCommand<Button>(
                        async (sender) =>
                        {
                            if (sender.Name == "buttonAccept")
                            {
                                bool result = await DataBaseService.PutUser(
                                    new User
                                    {
                                        Username = InputedLogin,
                                        Lname = InputedLname,
                                        IdRole = InputedIdRole
                                    }, InputedPassword
                                    );
                                if (!result) MessageBox.Show("Ошибка!");
                                else MessageBox.Show("Успешно!");

                            }
                            MainFrame.mainFrame.Navigate(new EditEmployeesPage(_enteredUser));
                        }
                        )
                    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
