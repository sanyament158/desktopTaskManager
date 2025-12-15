using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewScopePageViewModel : INotifyPropertyChanged
    {
        public AddNewScopePageViewModel(Model.User enteredUser)
        {
            _enteredUser = enteredUser;
        }
        //Fields & Properties
        private Model.User _enteredUser;
        private string _enteredName;
        public string EnteredName
        {
            get { return _enteredName; }
            set { _enteredName = value;
                OnPropertyChanged();
            }
        }

        //Commands 
        private AsyncRelayCommand<Button> _acceptCommand;
        public AsyncRelayCommand<Button> AcceptCommand
        {
            get {
                return _acceptCommand ?? (
                    _acceptCommand = new AsyncRelayCommand<Button>(
                        async (sender) =>
                        {
                            if (sender.Name == "buttonAccept")
                            {
                                
                                bool result =  await DataBaseService.PutScope(EnteredName);
                                if (!result) MessageBox.Show("Ошибка!");
                                else MessageBox.Show("Успешно!");
                                
                            }
                            MainFrame.mainFrame.Navigate(new EditScopesPage(_enteredUser));
                        }
                        )
                    ); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
