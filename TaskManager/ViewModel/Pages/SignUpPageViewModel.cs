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
using TaskManager.View.Pages;

namespace TaskManager.ViewModel.Pages
{
    public class SignUpPageViewModel: INotifyPropertyChanged
    {

        //Fields & Properties
        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
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
        private string _lname;
        public string Lname
        {
            get { return _lname; }
            set
            {
                _lname = value;
                OnPropertyChanged();
            }
        }

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
                                        Username = Login,
                                        Lname = this.Lname,
                                        IdRole = 2
                                    }, Password
                                    );
                                if (!result) MessageBox.Show("Ошибка!");
                                else MessageBox.Show("Успешно!");

                            }
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
    }
}
