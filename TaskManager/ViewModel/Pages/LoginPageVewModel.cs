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
using TaskManager.View.Pages.Users;


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
                                List<User> users = await DataBaseService.GetUsers();
                                User enteredUser = users.Where(u => u.Username == userResponseObj.username).First();
                                
                                if (userResponseObj.idRole == 1)
                                {
                                    MainFrame.mainFrame.Navigate(
                                        new MainPage(enteredUser)
                                    );
                                }
                                else
                                {
                                    MainFrame.mainFrame.Navigate(new UserPage(enteredUser));
                                }
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message.ToString(), "viewModel error"); }
                        },
                        obj => !(String.IsNullOrEmpty(_login) || String.IsNullOrEmpty(_password))
                        )
                    );
            }
        }
        private RelayCommand _signUpCommand;
        public RelayCommand SignUpCommand
        {
            get { return _signUpCommand ??
                    (
                    _signUpCommand = new RelayCommand(
                        (obj) =>
                        {
                            MainFrame.mainFrame.Navigate(new SignUpPage());
                        }
                        )
                    ); }
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
