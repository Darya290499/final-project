using System;
using System.Windows;
using System.Diagnostics;
namespace Diplom
{
   // App.Current.Properties["NameOfProperty"] = 5;
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            ProcessStartInfo psi;
            psi = new ProcessStartInfo("cmd.exe", @"/c sqllocaldb start diplom");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);

        }
        
        private void recruitment_Click(object sender, RoutedEventArgs e)
        {
            Add_new add_new = new Add_new();
            add_new.Show();
            this.Hide();
        }

        private void staff_Click(object sender, RoutedEventArgs e)
        {
            Staff staff = new Staff();
            staff.Show();
            this.Hide();
        }


        private void workers_Click(object sender, RoutedEventArgs e)
        {
            Workers workers = new Workers();
            workers.Show();
            this.Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ProcessStartInfo psi;
            psi = new ProcessStartInfo("cmd.exe", @"/c sqllocaldb stop diplom");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
            Environment.Exit(0);
        }
    }
}
