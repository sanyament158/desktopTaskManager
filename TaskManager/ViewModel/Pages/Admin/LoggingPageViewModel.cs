using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using TaskManager.Infrastucture.Network;
using TaskManager.Model;

namespace TaskManager.ViewModel.Pages.Admin
{
    public class LoggingPageViewModel: INotifyPropertyChanged
    {
        public LoggingPageViewModel(User enteredUser)
        {
            _ = InitializeAsync();
            _enteredUser = enteredUser;
        }
        // fields and props
        public List<LoggingRecord> Logs {  get; set; }
        private User _enteredUser;

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            Logs = await DataBaseService.GetLoggingRecords();
            OnPropertyChanged("Logs");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
