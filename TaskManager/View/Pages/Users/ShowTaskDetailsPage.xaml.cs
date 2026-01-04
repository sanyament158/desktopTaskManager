using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.Model;
using TaskManager.ViewModel.Pages.Users;

namespace TaskManager.View.Pages.Users
{
    /// <summary>
    /// Логика взаимодействия для ShowTaskDetailsPage.xaml
    /// </summary>
    public partial class ShowTaskDetailsPage : Page
    {
        public ShowTaskDetailsPage(User enteredUser, Model.Task enteredTask)
        {
            InitializeComponent();
            DataContext = new ShowTaskDetailsPageViewModel(enteredUser, enteredTask);
        }
    }
}
