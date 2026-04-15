using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    internal class ViewResponsibilitiesViewModel: INotifyPropertyChanged
    {
        public ViewResponsibilitiesViewModel(User _enteredUser) 
        {
            this._enteredUser = _enteredUser;
            System.Threading.Tasks.Task.Run(async () =>
            {
                List<Category> scopes = await DataBaseService.GetCategories();
                Scopes = scopes;
            }
            );
        }
        private User _enteredUser;

        private List<Category> _scopes;
        public List<Category> Scopes
        {
            get { return _scopes; }
            set { _scopes = value;
                OnPropertyChanged();
            }
        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                onCategoryChanged();
            }
        }
        private string _ratio;
        public string Ratio
        {
            get { return _ratio; }
            set { _ratio = value;
            OnPropertyChanged();}
        }
        private List<User> _users = new List<User>();
        public List<User> Users
        {
            get { return _users; }
            set { _users = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand
        {
            get {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(
                (obj) =>
                {
                    MainFrame.mainFrame.GoBack();
                })
                );
            }
        }
        private async void onCategoryChanged()
        {
            List<User> allUsers = await DataBaseService.GetUsers();
            List<User> sortedUsers = new List<User>();
            foreach ( User user in allUsers )
            {
                foreach (Category scope in user.Scopes)
                {
                    if (scope.Id == SelectedCategory.Id)
                    {
                        sortedUsers.Add(user);
                    }
                }
            }
            Users = sortedUsers;
            Ratio = $"{sortedUsers.Count()} сотрудник(-а) на одну область";
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
    }
}
