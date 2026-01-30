using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;
using TaskManager.View.Pages.Users;
using TaskManager.View.Pages;
using System.Diagnostics;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewTaskPageViewModel: INotifyPropertyChanged
    {
        public AddNewTaskPageViewModel(User enteredUser) 
        {
            RefreshScopesCommand.Execute(this);
            _owner = enteredUser.Lname;
            _enteredUser = enteredUser;
            _since = DateTime.Now;
            _deadline = DateTime.Now;
            
        }
        
        
        
        //Fields & Properties
        private User _enteredUser;

        private ObservableCollection<Category> _scopes;
        public ObservableCollection<Category> Scopes
        {
            get { return _scopes; }
            set
            {
                _scopes = value;
                OnPropertyChanged();
            }
        }
        private string _owner;
        public string Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                OnPropertyChanged();
            }
        }
        private int _idScope;
        public int IdScope
        {
            get { return _idScope; }
            set
            {
                _idScope = value;
                OnPropertyChanged();
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private DateTime _since;
        public string Since
        {
            get { return _since.ToString("yyyy-MM-dd"); }
            set
            {
                _since = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture);
                OnPropertyChanged();
            }
        }
        private DateTime _deadline;
        public string Deadline
        {
            get { return _deadline.ToString("yyyy-MM-dd"); }
            set
            {
                _deadline = DateTime.ParseExact(
                         value,
                         "yyyy-MM-dd",
                         System.Globalization.CultureInfo.InvariantCulture);
                OnPropertyChanged();
            }
        }

        
        
        //Commands
        private AsyncRelayCommand _refreshScopesCommand;
        public AsyncRelayCommand RefreshScopesCommand
        {
            get
            {
                return _refreshScopesCommand ?? (
                    _refreshScopesCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            var allScopes = await DataBaseService.GetCategories();

                            if (_enteredUser.Role.Id == 1)
                            {
                                Scopes = new ObservableCollection<Category>(allScopes);
                                return;
                            }
                            
                            var filteredScopes = new List<Category>();
                            foreach (var responsibility in _enteredUser.Scopes)
                            {
                                foreach (var scope in allScopes)
                                {
                                    if (scope.Id == responsibility.Id) filteredScopes.Add(scope);
                                }
                            }
                            Scopes = new ObservableCollection<Category>(filteredScopes);
                        }
                        )
                    );
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
                                User owner = null;
                                try
                                {
                                    if (_enteredUser.Role.Id != 1 && Owner != _enteredUser.Lname)
                                    {
                                        MessageBox.Show("В качестве владельца можно указать только себя!");
                                        return;
                                    }
                                    owner = await DataBaseService.GetUserByLname(Owner);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Нет такого пользователя!");
                                }
                                if (owner != null)
                                {
                                    Model.Task newTask = new Model.Task
                                    {
                                        Owner = new User { Id = (int)owner.Id },
                                        Status = new Status { Id = 1 },
                                        Title = this.Title,
                                        Scope = new Category { Id = this.IdScope },
                                        Since = this._since,
                                        Deadline = this._deadline
                                    };
                                    bool res = await DataBaseService.PutTask(newTask);
                                    if (res) MessageBox.Show("Успешно!");

                                    if (_enteredUser.Role.Id == 1) MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                                    else MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                }
                                else MessageBox.Show("Введенные поля неверны!");
                            }
                            else
                            {
                                if (_enteredUser.Role.Id != 1) MainFrame.mainFrame.Navigate(new UserPage(_enteredUser));
                                else MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                            }
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
