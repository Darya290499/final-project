using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Recruitment.xaml
    /// </summary>
    public partial class Recruitment : Window
    {
        void Show(bool check)
        {
            if(check==true)
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = true; ;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Visible;
                контактные_данныеDataGrid.Height = 246;
                //контактные_данныеDataGrid.Margin.Top = 114;
                контактные_данныеDataGrid.Margin = new Thickness(14,114,0,0);
            }
            else
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = false;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Hidden;
                контактные_данныеDataGrid.Height = 292;
                //контактные_данныеDataGrid.Margin.Top = 114;
                контактные_данныеDataGrid.Margin = new Thickness(14, 64, 0, 0);
            }
        }

        bool check = false;//, delete_check=false;//, check_phone = false, check_email = false;
        public Recruitment()
        {
            InitializeComponent();
         
            SqlCommand sql = new SqlCommand($"select * from Контактные_данные order by ФИО", connection);
            Add_col();
            Add_data(sql);
        }

        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\diplom; Initial Catalog = diplom; Integrated Security = True");

        void Add_col()
        {
            DataGridTextColumn column0 = new DataGridTextColumn();
            DataGridTextColumn column1 = new DataGridTextColumn();
            DataGridTextColumn column2 = new DataGridTextColumn();
            DataGridTextColumn column3 = new DataGridTextColumn();

            column1.IsReadOnly = column2.IsReadOnly = column3.IsReadOnly = true;

            column0.Header = "ID";
            column1.Header = "ФИО";
            column2.Header = "Номер телефона";
            column3.Header = "Электронная почта";

            контактные_данныеDataGrid.IsReadOnly = false;
            контактные_данныеDataGrid.Columns.Add(column1);
            контактные_данныеDataGrid.Columns.Add(column2);
            контактные_данныеDataGrid.Columns.Add(column3);

            column0.Binding = new Binding("id");
            column1.Binding = new Binding("fio");
            column2.Binding = new Binding("phone");
            column3.Binding = new Binding("email");
        }

        void Add_data(SqlCommand sql)
        {
            connection.Open();

            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    контактные_данныеDataGrid.Items.Add(new Contacts(Convert.ToInt32(reader[0]), reader[1] as string, reader[2] as string, reader[3] as string));
                }
            }
            connection.Close();
        }

        void Clean_data()
        {
            контактные_данныеDataGrid.Items.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        
        private void Контактные_данныеDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (контактные_данныеDataGrid.Items.Count > 0)
            {
                try
                {
                    Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];
                    fio.Text = row.fio;
                    phone.Text = row.phone;
                    email_address.Text = row.email;
                }
                catch { }
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string search_bar = fio_search.Text + "%";
             SqlCommand sql = new SqlCommand($"select * from Контактные_данные where ФИО like N'{search_bar}'", connection);
            Clean_data();
            Add_data(sql);
        }
        
        private void All_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand sql = new SqlCommand($"select * from Контактные_данные order by ФИО", connection);
            Clean_data();
            Add_data(sql);
        }

       

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw2 = new StreamWriter("Forms.txt");
            sw2.WriteLine("Recruitment");
            sw2.Close();

            Email em = new Email("Recruitment");
            em.Show();
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (контактные_данныеDataGrid.Items.Count > 0)
            {
                try
                {
                    /*DataRowView row = (DataRowView)контактные_данныеDataGrid.SelectedItems[0];
                    string str = Convert.ToString(row["Номер_телефона"]);*/


                }
                catch { }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Add_new add_new = new Add_new();
            add_new.Show();
            this.Hide();
        }

        private void Docs_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];
                Documents documents = new Documents(row.id);
                documents.Show();
                this.Hide();
            }
            catch { }
            
        }

   
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (контактные_данныеDataGrid.Items.Count > 0 && контактные_данныеDataGrid.SelectedIndex > -1)
            {
                Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];

                check = !check;
                /* if (check_phone == true && check_email == true) check = false;
                 else check = true; ;*/
                bool check_fio = false;
                string[] name = Convert.ToString(fio.Text).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (name.Length == 3 || name.Length == 2) check_fio = true;
                else check_fio = false;

                if (check == false)//true && check_phone == true && check_email == true)
                {
                    if (Regex.IsMatch(phone.Text, @"(\+7)[0-9]{6}[0-9]{4}") && Regex.IsMatch(email_address.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") && Regex.IsMatch(fio.Text, @"^[а-яА-Я\s-]+$") && check_fio == true)
                    {
                        SqlCommand q1 = new SqlCommand($"update Контактные_данные set ФИО=N'{fio.Text}', Номер_телефона=N'{phone.Text}', Электронная_почта=N'{email_address.Text}' where ID={row.id}", connection);
                        SqlCommand q2 = new SqlCommand($"update Сведения set ФИО=N'{fio.Text}' where ID={row.id}", connection);

                        connection.Open();
                        q1.ExecuteNonQuery();
                        q2.ExecuteNonQuery();
                        connection.Close();

                        MessageBox.Show("Изменения внесены");
                        Show(check);

                        SqlCommand sql = new SqlCommand($"select * from Контактные_данные order by ФИО", connection);
                        Clean_data();
                        Add_data(sql);
                    }
                    /* MessageBox.Show("Изменения внесены");
                 Show(check);*/
                    else
                    {
                        MessageBox.Show("Поля заполнены неверно"); check = true; Show(check);
                    }
                }
                else
                {
                    // MessageBox.Show("Поля заполнены неверно")
                    check = true; Show(check);
                }
            }
        }

       
    private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (контактные_данныеDataGrid.Items.Count > 0 && контактные_данныеDataGrid.SelectedIndex > -1)
            {

                Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];
                string name = row.fio;
                string message="";
                if (name[0]!= 'А' || name[0] != 'О' || name[0] != 'У' || name[0] != 'Ы' || name[0] != 'Э' || name[0] != 'И' || name[0] != 'Е' || name[0] != 'Ё' || name[0] != 'Ю' || name[0] != 'Я')
                    message = "Удалить всю информацию о " + name + "?";
                else message = "Удалить всю информацию об " + name + "?";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(message, "", buttons, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SqlCommand q1 = new SqlCommand($"delete from Контактные_данные where ID={row.id}", connection);
                    connection.Open();
                    q1.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Информация удалена");

                    SqlCommand sql = new SqlCommand($"select * from Контактные_данные order by ФИО", connection);
                    Clean_data();
                    Add_data(sql);
                }
            }
        }

    }
}
