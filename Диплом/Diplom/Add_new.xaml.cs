using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Add_new.xaml
    /// </summary>
    public partial class Add_new : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
        public Add_new()
        {
            InitializeComponent();

            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;
            sex.Items.Add("Ж");
            sex.Items.Add("М");
            l4.Content = l5.Content = l6.Content = "";
        }

        bool check_email = false, check_fio = false, check_phone = false;

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string[] name = Convert.ToString(fio.Text).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (name.Length == 3 || name.Length == 2) check_fio = true;
            else check_fio = false;

            if(check_email==true && check_fio==true && check_phone==true )
            {
                SqlCommand q1 = new SqlCommand($"insert into Контактные_данные(ФИО, Номер_телефона, Электронная_почта) values (N'{fio.Text}','{phone.Text}','{email.Text}'); select cast (scope_identity() as int)",connection);

                connection.Open();
                int id = (int)q1.ExecuteScalar();
               
                SqlCommand q2 = new SqlCommand($"insert into Сведения(ID,  Пол) values({id.ToString()},  N'{sex.SelectedItem}')", connection);
                 q2.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Информация внесена в базу данных");
                fio.Text = phone.Text = email.Text = "";
                l4.Content = l5.Content = l6.Content = "";
                sex.SelectedIndex = -1;
            }

        }

        private void Fio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l4!= null)
            {
                if (Regex.IsMatch(fio.Text, @"^[а-яА-Я\s-]+$"))
                {
                    l4.Content = " ";
                }
                else l4.Content = "Неверный формат";
            }
        }

        private void Recr_Click(object sender, RoutedEventArgs e)
        {
            Recruitment rec = new Recruitment();
            rec.Show();
            this.Hide();
        }

        private void Phone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l5 != null)
            {
                //Convert.ToDouble(phone.Text).ToString("+# (###) ###-##-##");
                if (Regex.IsMatch(phone.Text, @"(\+7)[0-9]{6}[0-9]{4}"))//@"^((\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{10}$"))
                {
                    l5.Content = " ";
                    check_phone = true;
                }
                else { l5.Content = "Неверный формат"; check_phone = false; }
            }
        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l6 != null)
            {
                if (Regex.IsMatch(email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))//s@ya.ru
                {
                    l6.Content = " ";
                    check_email = true;
                }
                else { l6.Content = "Неверный формат"; check_email = false; }
            }
        }

    }
}
