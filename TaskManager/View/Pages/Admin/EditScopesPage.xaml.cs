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

namespace TaskManager.View.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditScopesPage.xaml
    /// </summary>
    public partial class EditScopesPage : Page
    {
        public EditScopesPage(User enteredUser)
        {
            InitializeComponent();
        }
    }
}
