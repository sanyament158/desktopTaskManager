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
using TaskManager.View.Pages.Users;
using TaskManager.View.Pages.Admin;

namespace TaskManager.ViewModel.Pages.Admin;
public class UpdateTaskPageViewModel: INotifyPropertyChanged
{
    public UpdateTaskPageViewModel(User enteredUser, Model.Task task, List<Category> scopes)
    {

        _enteredUser = enteredUser;
        this.Task = task;
        _since = task.Since;
        _deadline = task.Deadline;
        _title = task.Title;
        _owner = task.Owner.Lname;
        _scopes = new ObservableCollection<Category>(scopes);
        _selectedScope = task.Scope;
        if (task.IdUserTaked != null && task.IdUserTaked != 0)
        {
            IsTaked = Visibility.Visible;
            _ = InitializeAsync();
        } else IsTaked = Visibility.Collapsed;
        
        if (task.Owner.Id == enteredUser.Id && task.Status.Id == 3)
            IsOwner = Visibility.Visible;
        else IsOwner = Visibility.Collapsed;
        

    }
    //Fields & Properties
    public Visibility IsOwner
    {
        get; set;
    }
    public Visibility IsTaked
    {
        get; set;
    }
    public string UserTakedText { get; set; }
    private Model.Task Task { get; set; }
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
    private Category _selectedScope;
    public Category SelectedScope
    {
        get { return _selectedScope; }
        set
        {
            _selectedScope = value;
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
                                    res = await DataBaseService.UpdateTaskById(Task.Id, new Model.Task
                                    {
                                        Id = Task.Id,
                                        Owner = newOwner,
                                        Title = this.Title,
                                        Scope = this.SelectedScope,
                                        Since = this._since,
                                        Deadline = this._deadline
                                    });
                                    await DataBaseService.PutLogging(_enteredUser.Id, $"Изменение задачи {Task.Title}");
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
    private AsyncRelayCommand _checkCommand;
    public AsyncRelayCommand CheckCommand
    {
        get 
        { return _checkCommand ?? (new AsyncRelayCommand(
            async (obj) =>
            {

                var answer = MessageBox.Show("Подтвердите проверку...", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (answer == MessageBoxResult.OK)
                {
                    bool res = await DataBaseService.UpdateFieldFromTableById("task", "idStatus", "2", Task.Id);
                    if (res) MessageBox.Show("Успешно!");
                }
                else return;
                MainFrame.mainFrame.Navigate(new MainPage(_enteredUser));
            }
            ));
        }
    }

    private async System.Threading.Tasks.Task InitializeAsync()
    {
        if (Task.IdUserTaked != null && Task.IdUserTaked != 0)
        {
            IsTaked = Visibility.Visible;

            var users = await DataBaseService.GetUsers();
            var user = users.FirstOrDefault(u => u.Id == Task.IdUserTaked);
            UserTakedText = user?.Lname ?? "Неизвестный пользователь";
        }
        else
        {
            IsTaked = Visibility.Collapsed;
            UserTakedText = "Не назначен";
        }
        OnPropertyChanged("UserTakedText");
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
