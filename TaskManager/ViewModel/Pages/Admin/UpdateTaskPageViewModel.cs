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
    public class UpdateTaskPageViewModel : INotifyPropertyChanged
    {
        public UpdateTaskPageViewModel(User enteredUser, TaskHumanReadable taskHR, Model.Task task)
        {

            RefreshScopesCommand.Execute(this);
            _enteredUser = enteredUser;
            this.task = task;
            this.taskHR = taskHR;

            _owner = taskHR.Owner;
            _scope = task.IdScope;
            _title = taskHR.Title;
            _since = task.Since;
            _deadline = task.Deadine;


        }
        //Fields & Properties
        private Model.Task task;
        private User _enteredUser;
        private TaskHumanReadable taskHR;

        private ObservableCollection<Category> _scopes;
        public ObservableCollection<Category> Scopes
        {
            get { return _scopes; }
            set { _scopes = value;
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
            set { _scope = value;
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
                                User user;
                                try
                                {
                                    User newOwner = await DataBaseService.GetUserByLname(Owner);
                                    MessageBox.Show(newOwner.Id.ToString());
                                    bool res = await DataBaseService.UpdateTaskById(task.Id, new Model.Task
                                    {
                                        Id = task.Id,
                                        IdOwner = (int)newOwner.Id,
                                        Title = this.Title,
                                        IdScope = this.Scope,
                                        Since = this.Since,
                                        Deadine = this.Deadline
                                    });
                                    MessageBox.Show(this.Since.ToString("yyyy-MM-dd"));
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
        public AsyncRelayCommand RefreshScopesCommand
        {
            get { return _refreshScopesCommand ?? (
                    _refreshScopesCommand = new AsyncRelayCommand(
                        async () => {
                            Scopes = new ObservableCollection<Category>(await DataBaseService.GetCategories());
                        }
                        )
                    ); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
