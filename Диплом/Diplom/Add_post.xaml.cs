using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Add_post.xaml
    /// </summary>
    public partial class Add_post : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);

        void DesDepList()
        {
            l_des_dep.Items.Clear();
            SqlCommand sql = new SqlCommand ( $"select distinct Конструкторское_бюро from Штатное_расписание", connection);
            connection.Open();
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    l_des_dep.Items.Add(reader[0] as string);
                }
            }
            connection.Close();
        }

        void DepList( SqlCommand sql)
        {
            l_dep.Items.Clear();
            //sql.CommandText = $"select distinct Подразделение from Штатное_расписание";
            connection.Open();
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    l_dep.Items.Add(reader[0] as string);
                }
            }
            connection.Close();
        }
        public Add_post()
        {
            InitializeComponent();
            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;
            l1.Content = l2.Content = l3.Content = l4.Content = l5.Content =l6.Content=" ";
            SqlCommand sql = new SqlCommand($"select distinct Подразделение from Штатное_расписание", connection);
            DesDepList();
            DepList(sql);
        }
        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(dep.Text!="" && des_dep.Text!="" && post.Text!="" && count.Text!="" && salary.Text!="" && access.Text != "")
            {
                if (l1.Content.ToString() == " " && l2.Content.ToString() == " " && l3.Content.ToString() == " " && l4.Content.ToString() == " " && l5.Content.ToString() == " " && l6.Content.ToString() == " ")
                {
                    bool check = false;
                    SqlCommand sql = new SqlCommand($"select Конструкторское_бюро, Подразделение, Должность from Штатное_расписание",connection);
                    connection.Open();
                    using (SqlDataReader reader = sql.ExecuteReader())
                    {
                        string string1 = des_dep.Text + "; " + dep.Text + "; " + post.Text;
                        while (reader.Read())
                        {
                            string string2 = reader[0] + "; " + reader[1] + "; " + reader[2];
                            if (string1 == string2) check = true;
                        }
                    }
                    connection.Close();
                    if (check == true) MessageBox.Show("Должность уже существует. Вы можете изменить информацию о ней в окне \"Штатное расписание\"");

                    else
                    {
                        sql.CommandText = $"insert into Штатное_расписание(Конструкторское_бюро, Подразделение, Должность, Количество_должностей, Количество_вакантных, Зарплата_полная_ставка, Форма_допуска) " +
                        $"values (N'{des_dep.Text}',N'{dep.Text}',N'{post.Text}',{count.Text},{count.Text},N'{salary.Text}',{access.Text})";
                        connection.Open();
                        sql.ExecuteNonQuery();
                        connection.Close();
                        string message = "Новая должность добавлена в систему. Очистить все поля?";
                        MessageBoxButton buttons = MessageBoxButton.YesNo;
                        MessageBoxResult result = MessageBox.Show(message, "", buttons, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            des_dep.Text = dep.Text = post.Text = salary.Text = count.Text = access.Text = "";
                            l1.Content = l2.Content = l3.Content = l4.Content = l5.Content = l6.Content = " ";
                            sql.CommandText=$"select distinct Подразделение from Штатное_расписание";
                            DesDepList();
                            DepList(sql);
                        }
                        else
                        {
                            int i1 = l_des_dep.SelectedIndex;
                            int i2 = l_des_dep.SelectedIndex;
                            post.Text = salary.Text = count.Text = access.Text = "";
                            l1.Content = l2.Content = l3.Content = l4.Content = l5.Content = l6.Content = " ";
                            sql.CommandText = $"select distinct Подразделение from Штатное_расписание";
                            DesDepList();
                            DepList(sql);
                            l_des_dep.SelectedIndex = i1;
                            l_dep.SelectedIndex = i2;
                        }
                    }
                }

                else MessageBox.Show("Некоторые поля заполнены некорректно");
            }
            else
            {
                MessageBox.Show("Не все поля заполнены");
            }
        }

        private void Des_dep_TextChanged(object sender, TextChangedEventArgs e)
        {
            l_des_dep.SelectedIndex = -1;
            if (l1 != null)
            {
                if (Regex.IsMatch(des_dep.Text, @"^[А-Я0-9\s-]+$"))
                {
                    l1.Content = " ";
                }
                else l1.Content = "Поле заполнено некорректно";
            }
        }

        private void Dep_TextChanged(object sender, TextChangedEventArgs e)
        {
            l_dep.SelectedIndex = -1;
            if (l2 != null)
            {
                if (Regex.IsMatch(dep.Text, @"^[а-яА-Я0-9\s-,]+$"))
                {
                    l2.Content = " ";
                }
                else l2.Content = "Поле заполнено некорректно";
            }
        }

        private void Post_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l3 != null)
            {
                if (Regex.IsMatch(post.Text, @"^[а-яА-Я0-9-\s]+$"))
                {
                    l3.Content = " ";
                }
                else l3.Content = "Поле заполнено некорректно";
            }
        }

        private void Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l4 != null)
            {
                if (Regex.IsMatch(count.Text, @"^[0-9]+$"))
                {
                    l4.Content = " ";
                }
                else l4.Content = "Поле заполнено некорректно";
            }
        }

        private void Salary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l5 != null)
            {
                if (Regex.IsMatch(salary.Text, @"^[0-9]+$"))
                {
                    l5.Content = " ";
                }
                else l5.Content = "Поле заполнено некорректно";
            }
        }
        private void Access_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (l6 != null)
            {
                if (Regex.IsMatch(access.Text, @"^[1-3]+$"))
                {
                    if(Convert.ToInt32(access.Text)>3)
                        l6.Content = "Неверная форма допуска";
                    else l6.Content = " ";
                }
                else l6.Content = "Поле заполнено некорректно";
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Staff st = new Staff();
            st.Show();
        }

        private void L_des_dep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            l_dep.SelectedIndex = -1;dep.Text = ""; l2.Content = "";
            SqlCommand sql = new SqlCommand($"", connection);
            if (l_des_dep.SelectedIndex != -1)
            {
                des_dep.Text = l_des_dep.Items[l_des_dep.SelectedIndex].ToString();
                l_des_dep.Text = des_dep.Text;
                sql.CommandText =  $"select distinct Подразделение from Штатное_расписание where Конструкторское_бюро=N'{l_des_dep.SelectedItem}'";
            }
            else sql.CommandText = $"select distinct Подразделение from Штатное_расписание ";//l_dep.IsEnabled = false;
            DepList(sql);
        }

        private void L_dep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (l_dep.SelectedIndex != -1) { dep.Text = l_dep.Items[l_dep.SelectedIndex].ToString(); l_dep.Text = dep.Text; }
        }
    }
}
