using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
using TaskManager.ViewModel.Pages.Admin;
using TaskManager.ViewModel.Pages.Users;

namespace TaskManager.View.Pages.Users
{
    /// <summary>
    /// Логика взаимодействия для FinishedTasksPage.xaml
    /// </summary>
    public partial class FinishedTasksPage : Page
    {
        public FinishedTasksPage(User enteredUser, List<Model.Task> tasks, List<Category> scopes)
        {
            InitializeComponent();
            DataContext = new FinishedTasksPageViewModel(enteredUser, tasks, scopes);
        }
    }
}
