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

namespace TaskManager.ViewModel.Pages.Admin
{
    public class AddNewTaskPageViewModel: INotifyPropertyChanged
    {
        public AddNewTaskPageViewModel(User enteredUser) 
        {
            _enteredUser = enteredUser;
            _since = DateTime.Now;
            _deadline = DateTime.Now;
            RefreshScopesCommand.Execute(this);
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
        private int _scope;
        public int Scope
        {
            get { return _scope; }
            set
            {
                _scope = value;
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
        public DateTime Since
        {
            get { return _since; }
            set
            {
                _since = value;
                OnPropertyChanged();
            }
        }
        private DateTime _deadline;
        public DateTime Deadline
        {
            get { return _deadline; }
            set
            {
                _deadline = value;
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
                        async () => {
                            Scopes = new ObservableCollection<Category>(await DataBaseService.GetCategories());
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
                                        IdOwner = (int)owner.Id,
                                        IdStatus = 1,
                                        Title = this.Title,
                                        IdScope = Scope,
                                        Since = this.Since,
                                        Deadine = this.Deadline
                                    };
                                    bool res = await DataBaseService.PutTask(newTask);
                                    if (res) MessageBox.Show("Успешно!");
                                }
                                else MessageBox.Show("Введенные поля неверны!");
                            }
                            MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
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
