using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages
{
    public class LoginPageVewModel : INotifyPropertyChanged
    {
        private string _login;
        public string Login
        {
            get { return _login; }
            set 
            { _login = value;
                OnPropertyChanged();
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                    (
                    _loginCommand = new RelayCommand(
                        async obj =>
                        {
                            UserRequest userObj = new UserRequest
                            {
                                username = _login,
                                password = _password
                            };
                            try
                            {
                                UserResponse userResponseObj = await DataBaseService.AuthorizeUser(userObj);
                                MessageBox.Show(userResponseObj.username);
                                if (userResponseObj.idRole == 1)
                                {
                                    MainFrame.mainFrame.Navigate(
                                        new MainPage(
                                            new User { Username = userResponseObj.username, IdRole = userResponseObj.idRole }
                                            )
                                        );
                                }
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "viewModel error"); }
                        },
                        obj => !(String.IsNullOrEmpty(_login) || String.IsNullOrEmpty(_password))
                        )
                    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
