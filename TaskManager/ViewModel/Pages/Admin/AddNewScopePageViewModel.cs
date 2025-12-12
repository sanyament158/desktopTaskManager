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

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewScopePageViewModel : INotifyPropertyChanged
    {
        public AddNewScopePageViewModel(EditScopesPageViewModel senderContext)
        {
            senderContext = this.senderContext;
        }
        //Fields & Properties
        private EditScopesPageViewModel senderContext;
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
                                    if (!result) MessageBox.Show("query error occured");
                                    else
                                    {
                                        senderContext.RefreshScopesCommand.Execute(senderContext);
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
