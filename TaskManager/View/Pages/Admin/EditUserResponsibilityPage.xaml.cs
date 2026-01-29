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
using TaskManager.ViewModel.Pages.Admin;

namespace TaskManager.View.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditUserResponsibilityPage.xaml
    /// </summary>
    public partial class EditUserResponsibilityPage : Page
    {
        public EditUserResponsibilityPage(User enteredUser, User selectedUser, List<Category> availScopes, string pass)
        {
            InitializeComponent();
            DataContext = new EditUserResponsibilityPageViewModel(enteredUser, selectedUser, availScopes, pass);
        }
    }
}
