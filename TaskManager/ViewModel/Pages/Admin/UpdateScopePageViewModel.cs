using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using TaskManager.Model;
using TaskManager.Infrastucture.Network;
using TaskManager.Infrastucture.Navigation;
using System.Windows;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class UpdateScopePageViewModel: INotifyPropertyChanged
    {
        public UpdateScopePageViewModel(User enteredUser, Category selectedScope)
        {
            _selectedCategory = selectedScope;
            _enteredUser = enteredUser;

            _enteredName = selectedScope.Name;
        }
        //Fields & Properties
        private Category _selectedCategory;
        private User _enteredUser;
        private string _enteredName;
        public string EnteredName
        {
            get { return _enteredName; }
            set
            {
                _enteredName = value;
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
                                bool result = await DataBaseService.UpdateFieldFromTableById("scope", "name", _enteredName, _selectedCategory.Id);
                                if (!result) MessageBox.Show("Ошибка!");
                                else MessageBox.Show("Успешно!");

                            }
                            MainFrame.mainFrame.Navigate(new EditScopesPage(_enteredUser));
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
