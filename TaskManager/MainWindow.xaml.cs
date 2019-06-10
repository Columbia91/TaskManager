using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<ProcessDetails> details = new ObservableCollection<ProcessDetails>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowProcesses();
        }

        private void ShowProcesses()
        {
            var allProcess = from pr in Process.GetProcesses()
                             select pr;

            foreach (var process in allProcess)
            {
                details.Add(new ProcessDetails
                {
                    ProcessName = process.ProcessName,
                    ProcessId = process.Id,
                    State = process.Responding ? "Выполняется" : "Остановлено",
                    User = GetUserName(process.Id),
                    Memory = GetUsedMemory(process),
                    Description = GetProcessDescription(process.Id)
                });
            }
            dataGrid.ItemsSource = details;
        }
        
        private string GetUserName(int Id)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            var process = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

            if (process != null)
            {
                string[] args = new string[] { string.Empty, string.Empty };
                int result = Convert.ToInt32(process.InvokeMethod("GetOwner", args));
                if (result == 0)
                {
                    return args[0];
                }
            }
            
            return "Система";
        }
        private string GetUsedMemory(Process process)
        {
            PerformanceCounter PC = new PerformanceCounter();

            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = process.ProcessName;
            
            try
            {
                return (PC.NextValue() / 1024).ToString() + " KB";
            }
            catch (Exception) {}

            return "Нет данных";
        }
        private string GetProcessDescription(int Id)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            var process = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

            if(process != null)
            { 
                try
                {
                    var path = process["ExecutablePath"];
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(path.ToString());
                    return fileVersionInfo.FileDescription;
                }
                catch (Exception) {}
            }
            return "Нет данных";
        }
        
        private void KillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var detail = (ProcessDetails)dataGrid.SelectedItem;
                var process = Process.GetProcessesByName(detail.ProcessName).FirstOrDefault();
                process.Kill();
                details.Remove((ProcessDetails)dataGrid.SelectedItem);
            }
            catch (Exception) {}
        }
    }
}