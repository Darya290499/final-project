using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Documents.xaml
    /// </summary>
    public partial class Documents : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);

        void Update(string arg, int val)
        {
            SqlCommand sql= new SqlCommand($"update Сведения set {arg}={val} where ID={id}",connection);
            connection.Open();
            sql.ExecuteNonQuery();
            connection.Close();
        }

        System.Windows.Controls.CheckBox [] checks = new System.Windows.Controls.CheckBox[16];
        bool[] val = new bool[16];
        int status;
        int id=0;
        void SetChecks()
        {
            checks[0] = ch0; checks[1] = ch1; checks[2] = ch2; checks[3] = ch3;
            checks[4] = ch4; checks[5] = ch5; checks[6] = ch6; checks[7] = ch7;
            checks[8] = ch8; checks[9] = ch9; checks[10] = ch10; checks[11] = ch11;
            checks[12] = ch12; checks[13] = ch13; checks[14] = ch14; checks[15] = ch15;
        }

        Visibility check_value(string sex, bool []val)
        {
            if(sex=="Ж")
            {
                int check1=0, check2=0, check3=0;
                for (int i = 0; i < 3; i++) if (val[i] == true) check1++;
                for (int i = 4; i < 7; i++) if (val[i] == true) check2++;
                for (int i = 9; i < 16; i++) if (val[i] == true) check3++;
                if (check1 == 3 && check2 == 3 && check3 == 7 && decree==false) return Visibility.Visible;
                else return Visibility.Hidden;
            }
            else
            {
                int check1 = 0, check2 = 0;
                for (int i = 0; i < 7; i++) if (val[i] == true) check1++;
                for (int i = 9; i < 16; i++) if (val[i] == true) check2++;
                if (check1 == 7 && check2 == 7 && decree == false) return Visibility.Visible;
                else return Visibility.Hidden;
            }
              
        }

        string name = "";
        string sex = "";
        bool decree = false;
        string from = "";
        public Documents(int ID, string all_name, string form)
        {
            InitializeComponent();
            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            SetChecks();
            id = ID;
            from = form;
            
            SqlCommand query = new SqlCommand($"select* from Сведения where ID={ID}", connection);

            connection.Open();
            using(SqlDataReader reader = query.ExecuteReader())
            {
                while(reader.Read())
                {
                    fio.Content = name = all_name;
                    
                    sex = reader[1] as string;
                    for (int i = 2; i < 18; i++)
                    {
                        if (Convert.ToString(reader[i]).ToLower() == "true") { checks[i - 2].IsChecked = true; val[i - 2] = true; }
                        else { checks[i - 2].IsChecked = false; val[i - 2] = false; }
                    }
                    if (Convert.ToString(reader[18]).ToLower() == "true") decree = true;
                }
                //  if (Convert.ToString(reader[19]).ToLower() != "") decree = Convert.ToBoolean(reader[19]);
                //string znach = Convert.ToString(reader[18]);
            }
            connection.Close();

            if (decree == false) new_worker.Visibility = check_value(sex, val);
            else
            {
                for(int i=0;i<16;i++)
                {
                    if (checks[i].IsChecked == true) checks[i].IsEnabled = false;
                }
                new_worker.Visibility = Visibility.Hidden;
            }
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(from== "Recruitment")
            {
                Recruitment recr = new Recruitment();
                recr.Show();
            }
            else
            {
                Workers work = new Workers();
                work.Show();
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            int i= 0;
            for (; i < 16; i++)
                if (checks[i].IsChecked != val[i]) { status = i; break; }

            switch (status)
            {
                case 0:
                    Update(Копия_паспорта.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 1:
                    Update(Диплом.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 2:
                    Update(Трудовая_книжка.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 3:
                    Update(Военный_билет.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 4:
                    Update(Фото.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 5:
                    Update(Мед_справка.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 6:
                    Update(Итоги_фл.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 7:
                    Update(Документы_УчС.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 8:
                    Update(СНИЛС.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 9:
                    Update(ИНН.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 10:
                    Update(Удостоверение_ветерана.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 11:
                    Update(Заявление_на_прием.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 12:
                    Update(Анкета.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 13:
                    Update(Заявление_о_зп.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 14:
                    Update(Обязательство.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
                case 15:
                    Update(Согласие.Name, Convert.ToInt32(checks[status].IsChecked));
                    val[status] = (bool)checks[status].IsChecked;
                    new_worker.Visibility = check_value(sex, val);
                    break;
            }
        }

        private void New_worker_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand sql = new SqlCommand($"select count(*) from Штатное_расписание where Количество_вакантных > 0", connection);
            connection.Open();
            int count = (int)sql.ExecuteScalar();
            connection.Close();
            if (count > 0)
            {
                Add_worker add_worker = new Add_worker(name,id, from);
                add_worker.Show();
                this.Hide();
           
            }
            else System.Windows.MessageBox.Show("Нет вакантных должностей");

        }
    }
}
