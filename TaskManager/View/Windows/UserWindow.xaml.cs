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
using System.Windows.Shapes;
using TaskManager.Infrastucture.Navigation;
using TaskManager.Model;
using TaskManager.ViewModel.Windows;
using TaskManager.View.Pages;
using TaskManager.View.Pages.Users;

namespace TaskManager.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow(User enteredUser)
        {
            InitializeComponent();
            MainFrame.userWindow = this;
            MainFrame.mainFrame = userWindowFrame;
            MainFrame.mainFrame.Navigate(new UserPage(enteredUser));
            DataContext = new UserWindowViewModel(enteredUser);
        }
    }
}
