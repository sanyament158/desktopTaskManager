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

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewScopePageViewModel : INotifyPropertyChanged
    {
        //Fields & Properties
        private string _enteredName;
        public string EnteredName
        {
            get { return _enteredName; }
            set { _enteredName = value;
                OnPropertyChanged();
            }
        }

        //Commands 
        private RelayCommand _acceptCommand;
        public RelayCommand AcceptCommand
        {
            get {
                return _acceptCommand ?? (
                    _acceptCommand = new RelayCommand(
                        (obj) =>
                        {
                            Button sender = obj as Button;
                            if (sender.Name == "buttonAccept")
                            {
                                try
                                {
                                    // database request
                                    MessageBox.Show("db req");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            MainFrame.mainFrame.GoBack();
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
