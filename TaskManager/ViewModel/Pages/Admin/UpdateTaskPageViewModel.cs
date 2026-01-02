using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class UpdateTaskPageViewModel //: INotifyPropertyChanged
    {
        public UpdateTaskPageViewModel(User enteredUser, Model.Task task)
        {

            RefreshScopesCommand.Execute(this);
            _enteredUser = enteredUser;
        }
        //Fields & Properties
        private Model.Task task;
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
        private Category _scope;
        public Category Scope
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
        public string Since
        {
            get { return _since.ToString("yyyy-MM-dd"); }
            set
            {
                _since = DateTime.ParseExact(
                    value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture
                );
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
                    System.Globalization.CultureInfo.InvariantCulture
                );
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
                                bool res = false;
                                User user;
                                try
                                {
                                    User newOwner = await DataBaseService.GetUserByLname(Owner);
                                    try
                                    {
                                        res = await DataBaseService.UpdateTaskById(task.Id, new Model.Task
                                        {
                                            Id = task.Id,
                                            Owner = newOwner,
                                            Title = this.Title,
                                            Scope = this.Scope,
                                            Since = this._since,
                                            Deadline = this._deadline
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Формат даты некорректен");
                                    }
                                    if (res) { MessageBox.Show("Успешно!"); }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
                        }
                        )
                    );
            }
        }
        private AsyncRelayCommand _refreshScopesCommand;
        public IAsyncRelayCommand RefreshScopesCommand
        {
            get {
                return _refreshScopesCommand ?? 
                    (_refreshScopesCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            Scopes = new ObservableCollection<Category>(await DataBaseService.GetCategories());
                        }
                        )
                    );
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
