using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TaskManager.Model;
using TaskManager.Infrastucture.Network;
using System.Collections.ObjectModel;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            RefreshCategoriesCommand.Execute(this);
        }
        // fields & properties
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
            get
            {
                return _categories;
            }
        }

        // commands

        private AsyncRelayCommand _refreshCategoriesCommand;
        public IAsyncRelayCommand RefreshCategoriesCommand
        {
            get {
                return _refreshCategoriesCommand ?? 
                    (_refreshCategoriesCommand = new AsyncRelayCommand(
                        async () =>
                        {
                            MessageBox.Show("command is start");
                            Categories = new ObservableCollection<Category>(await DataBaseService.GetCategories());
                        }
                        )
                    );
                }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
