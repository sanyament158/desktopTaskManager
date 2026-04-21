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
using TaskManager.ViewModel.Pages.Users;
using TaskManager.Model;

namespace TaskManager.View.Pages.Users
{
    /// <summary>
    /// Логика взаимодействия для MyTasksPage.xaml
    /// </summary>
    public partial class MyTasksPage : Page
    {
        public MyTasksPage(User enteredUser)
        {
            InitializeComponent();
            DataContext = new MyTasksPageViewModel(enteredUser);
        }
    }
}
