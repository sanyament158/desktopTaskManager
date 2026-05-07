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
using TaskManager.View.Pages;
using TaskManager.ViewModel.Windows;

namespace TaskManager.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            MainFrame.mainFrame = loginFrame;
            MainFrame.mainFrame.Navigate(new LoginPage());
            MainFrame.loginWindow = this;
        }
    }
}
