using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Documents.xaml
    /// </summary>
    public partial class Documents : Window
    {
        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\diplom; Initial Catalog = diplom; Integrated Security = True");

        CheckBox [] checks = new CheckBox[16];

        void Update(SqlCommand query)
        {
            connection.Open();
            query.ExecuteNonQuery();
            connection.Close();
        }
        
        void SetChecks()
        {
            checks[0] = ch0; checks[1] = ch1; checks[2] = ch2; checks[3] = ch3;
            checks[4] = ch4; checks[5] = ch5; checks[6] = ch6; checks[7] = ch7;
            checks[8] = ch8; checks[9] = ch9; checks[10] = ch10; checks[11] = ch11;
            checks[12] = ch12; checks[13] = ch13; checks[14] = ch14; checks[15] = ch3;
        }

        string sex = "";
        public Documents(int ID)
        {
            InitializeComponent();
            //fio.Content = ID;
            SetChecks();

            SqlCommand query = new SqlCommand($"select* from Сведения where ID={ID}", connection);
            List<string> prob = new List<string>();
             connection.Open();

             using(SqlDataReader reader = query.ExecuteReader())
             {
                 while(reader.Read())
                 {
                    fio.Content = reader[1] as string;
                    sex = reader[2] as string;
                    for (int i = 3; i < 19; i++)
                    {
                        if (Convert.ToString(reader[i]).ToLower() == "true") checks[i - 3].IsChecked = true;
                        else checks[i-3].IsChecked = false;
                    }
                     //for (int i = 0; i < 16; i++) checks[i].IsChecked = Convert.ToBoolean(reader[i + 3]);
                 }
             }
             connection.Close();

           // checks[0].IsChecked = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Recruitment recr = new Recruitment();
            recr.Show();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
