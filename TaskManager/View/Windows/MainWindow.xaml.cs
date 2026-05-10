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
using TaskManager.View.Pages;
using TaskManager.View.Pages.Admin;
using TaskManager.ViewModel.Pages;
using TaskManager.ViewModel.Windows;

namespace TaskManager.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(User enteredUser)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(enteredUser);
            MainFrame.mainWindow = this;
            mainWindowFrame.Navigate(new MainPage(enteredUser));
            MainFrame.mainFrame = this.mainWindowFrame;
        }
    }
}
