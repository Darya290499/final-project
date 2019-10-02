using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;
/*using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;*/

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
        }

       // SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\kurs; Initial Catalog = kurs; Integrated Security = True");


       

       /* void SaveTable()
        {
           /* string path = System.IO.Directory.GetCurrentDirectory() + @"\"+"Save_Channel.xlsx";
            Excel.Application excelapp = new Excel.Application();
            Excel.Workbook workbook = excelapp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.ActiveSheet;
            int row = сведенияDataGrid.Items.Count;
            int col = сведенияDataGrid.Columns.Count();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    сведенияDataGrid.SelectedIndex = i;
                   worksheet.Rows[i].Columns[j] = сведенияDataGrid.Items[i].
                }
            }
            excelapp.AlertBeforeOverwriting = false;
            workbook.SaveAs(path);
            excelapp.Quit();
        }*/

        private void recruitment_Click(object sender, RoutedEventArgs e)
        {
            Recruitment form = new Recruitment();
            form.Show();
            this.Hide();
        }

        private void staff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void workers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
