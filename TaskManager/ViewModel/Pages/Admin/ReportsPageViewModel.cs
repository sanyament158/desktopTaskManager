using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaskManager.ViewModel.Pages.Admin
{
    /// <summary>
    /// this class should calling a report service. 
    /// params for reports: 
    ///    (4 params) date, owner, status, scope.
    ///  users can: 
    ///     get report taked tasks. all tasks having user's scope. without the finished status.
    ///  admins can: 
    ///     to filter all props and create a report. and get the all finishedTasks.
    ///     
    /// scheme: 
    ///    command -> call the method(model objects)
    ///             <- get document object
    ///    saving .docx document 
    /// </summary>
    public class ReportsPageViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
