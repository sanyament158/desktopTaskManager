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
                            User newuser = new User
                            {
                                Username = InputedLogin,
                                Lname = InputedLname,
                                Role = new Role { Id = InputedIdRole }
                            };

                            bool result = await DataBaseService.PutUser(newuser, InputedPassword);


                                if (!result) MessageBox.Show("Ошибка!");
                                else MessageBox.Show("Успешно!");
                            await DataBaseService.PutLogging(_enteredUser.Id, $"Создан пользователь {newuser.Fname}");
                            MainFrame.mainFrame.Navigate(new EditEmployeesPage(_enteredUser));
                        }
                        )
                    );
            }
        }
        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(
                (obj) =>
                {
                    MainFrame.mainFrame.GoBack();
                })
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
