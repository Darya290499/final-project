using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Add_worker.xaml
    /// </summary>
    public partial class Add_worker : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
        int ID = 0;
        string fio = "";
        string from = "";
        public Add_worker(string name_worker, int id, string form)
        {
            InitializeComponent();
            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;
            ID = id;
            from = form;
            fio = name_worker;
            if (form == "Recruitment")
            {
                this.Title = "Новый сотрудник";
                name.Content = "Новый сотрудник " + name_worker;
            }
            else
            {
                this.Title = "Смена должности";
                name.Content = "Сотрудник " + name_worker;
            }
            
            
            SqlCommand sql = new SqlCommand($"select distinct Конструкторское_бюро from Штатное_расписание where Количество_вакантных > 0", connection);
            connection.Open();
            
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    des_dep.Items.Add(reader[0] as string);
                }
            }
            connection.Close();
            l1.Visibility = Visibility.Hidden;
        }
        

        List<int> id_post = new List<int>();
      
        private void des_dep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SqlCommand sql = new SqlCommand($"select distinct Подразделение from Штатное_расписание where Количество_вакантных > 0 and Конструкторское_бюро=N'{des_dep.SelectedItem}'", connection);
            dep.IsEnabled = true;
            connection.Open();
            
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    dep.Items.Add(reader[0] as string);
                }
            }
            connection.Close();
            dep.SelectedIndex = post.SelectedIndex = -1;
        }

        private void dep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dep.SelectedIndex != -1)
            {
                SqlCommand sql = new SqlCommand($"select ID, Должность from Штатное_расписание where Количество_вакантных > 0 and Конструкторское_бюро=N'{des_dep.SelectedItem}' and Подразделение=N'{dep.SelectedItem}'", connection);
                post.IsEnabled = true;
                connection.Open();

                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id_post.Add(Convert.ToInt32(reader[0]));
                        post.Items.Add(reader[1] as string);
                    }
                }
                connection.Close();
                post.SelectedIndex = -1;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (number.Text != "" && des_dep.SelectedIndex != -1 && dep.SelectedIndex != -1 && post.SelectedIndex != -1 && Regex.IsMatch(bet.Text, @"^[0-9.,]+$")) 
            {
                if (Convert.ToDouble(bet.Text.Replace('.', ',')) > bet_count || Convert.ToDouble(bet.Text.Replace('.', ',')) == 0)
                    MessageBox.Show("Ставка указана неверно");
                else
                {
                    if (from == "Recruitment")
                    {
                        SqlCommand q1 = new SqlCommand($"update Сведения set Приказ_зачисление=1 where ID={ID}", connection);
                        connection.Open();
                        q1.ExecuteNonQuery();

                        q1.CommandText = $"select Количество_вакантных, Зарплата_полная_ставка from Штатное_расписание  where ID={id_post[post.SelectedIndex]}";

                        double count = 0;
                        double salary = 0.0;

                        using (SqlDataReader reader = q1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                count = Convert.ToDouble(reader[0]);
                                if (reader[1] as string != null) salary = Convert.ToDouble(reader[1]);
                            }
                        }
                       

                        count-= Math.Round(Convert.ToDouble(bet.Text.Replace('.', ',')),2);

                        salary *= Math.Round(Convert.ToDouble(bet.Text.Replace('.', ',')), 2);
                        q1.CommandText = $"update Штатное_расписание set Количество_вакантных={count.ToString().Replace('.', ',')} where ID={id_post[post.SelectedIndex]}";
                        q1.ExecuteNonQuery();

                        q1.CommandText = $"insert into Список_сотрудников(ID, Приказ_зачисление) values ({ID}, N'{number.Text}')";
                        q1.ExecuteNonQuery();

                        q1.CommandText = $"insert into Список_должностей(ID_сотрудника, ID_должности, Ставка, Оклад) values ({ID},{id_post[post.SelectedIndex]},{bet.Text.Replace(',', '.')},{salary})";
                        q1.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("Сотрудник зарегистрирован");
                        this.Close();
                    }
                    else
                    {
                        SqlCommand q1 = new SqlCommand($"select ID_должности from Список_должностей where ID_сотрудника={ID}", connection);
                        connection.Open();
                        int post_id= (int)q1.ExecuteScalar();

                        q1.CommandText = $"select Ставка from Список_должностей where ID_сотрудника={ID}";

                        double rate = (double)q1.ExecuteScalar();

                        q1.CommandText = $"select Количество_вакантных from Штатное_расписание  where ID={post_id}";

                        double count = (double)q1.ExecuteScalar();
                        count += rate; ;
                        q1.CommandText = $"update  Штатное_расписание set Количество_вакантных={count} where ID={post_id}";
                        q1.ExecuteNonQuery();

                        q1.CommandText = $"select Количество_вакантных, Зарплата_полная_ставка from Штатное_расписание  where ID={id_post[post.SelectedIndex]}";

                        double salary = 0.0;

                        using (SqlDataReader reader = q1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                count = Convert.ToDouble(reader[0]);
                                if (reader[1] as string != null) salary = Convert.ToDouble(reader[1]);
                            }
                        }
                        count -= Math.Round(Convert.ToDouble(bet.Text.Replace('.', ',')), 2);

                        salary *= Math.Round(Convert.ToDouble(bet.Text.Replace('.', ',')), 2);
                        q1.CommandText = $"update Штатное_расписание set Количество_вакантных={count.ToString().Replace(',', '.')} where ID={id_post[post.SelectedIndex]}";
                        q1.ExecuteNonQuery();

                        q1.CommandText = $"update Список_должностей set ID_должности={id_post[post.SelectedIndex]}, Ставка={bet.Text.Replace(',', '.')}, Оклад={salary}, Приказ_назначения=N'{number.Text}' where ID_сотрудника={ID}";
                        q1.ExecuteNonQuery();

                        connection.Close();

                        MessageBox.Show("Должность изменена");
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Некоторые поля не заполнены или заполнены неверно");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (from == "Workers")
            {
                Workers work = new Workers();
                work.Show();
            }
            else
            {
                Documents docs = new Documents(ID, fio, from);
                docs.Show();
            }
        }

        double bet_count = 0;
        private void Post_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (post.SelectedIndex == -1) l1.Visibility = Visibility.Hidden;
            else 
            {
               
                SqlCommand q1 = new SqlCommand($"select Количество_вакантных, Зарплата_полная_ставка from Штатное_расписание  where ID={id_post[post.SelectedIndex]}", connection);

                double count = 0;
                connection.Open();
                count = (double)q1.ExecuteScalar();
                connection.Close();

                if (count > 1) { l1.Content = "*ставка не выше 1"; bet_count = 1; }
                else { l1.Content = "*ставка не выше " + count.ToString(); bet_count = count; }
                l1.Visibility = Visibility.Visible;
            }
        }
    }
}

