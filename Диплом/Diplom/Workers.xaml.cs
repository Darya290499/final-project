using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using MessageBox = System.Windows.MessageBox;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Workers.xaml
    /// </summary>
    public partial class Workers : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);

        void Show(bool check)
        {
            if (check == true)
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = true; ;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Visible;
                список_сотрудниковDataGrid.Height = 395;
                //контактные_данныеDataGrid.Margin.Top = 114;
                список_сотрудниковDataGrid.Margin = new Thickness(14, 114, 0, 0);
            }
            else
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = false;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Hidden;
                список_сотрудниковDataGrid.Height = 445;
                //контактные_данныеDataGrid.Margin.Top = 114;
                список_сотрудниковDataGrid.Margin = new Thickness(14, 64, 0, 0);
            }
        }

        void Show1(bool check)
        {
            if (check == true)
            {
                birth.IsEnabled = settlement.IsEnabled = street.IsEnabled = house.IsEnabled = flat.IsEnabled = true;
            }
            else
            {
                birth.IsEnabled = settlement.IsEnabled = street.IsEnabled = house.IsEnabled = flat.IsEnabled = false;
            }
        }

        bool check = false;
        bool check1 = false;
        void Add_col()
        {

            DataGridTextColumn[] columns = new DataGridTextColumn[4];
            for (int i = 0; i < 4; i++)
            {
                columns[i] = new DataGridTextColumn();
                columns[i].IsReadOnly = true;
            }

            columns[0].Header = "ID";
            columns[1].Header = "ФИО";
            columns[2].Header = "Номер телефона";
            columns[3].Header = "Электронная почта";

            for (int i = 1; i < 4; i++)
            {
                список_сотрудниковDataGrid.Columns.Add(columns[i]);
            }

            columns[0].Binding = new System.Windows.Data.Binding("id");
            columns[1].Binding = new System.Windows.Data.Binding("fio");
            columns[2].Binding = new System.Windows.Data.Binding("phone");
            columns[3].Binding = new System.Windows.Data.Binding("email");
        }

        void Add_data(SqlCommand sql)
        {
            connection.Open();

            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    список_сотрудниковDataGrid.Items.Add(new Contacts(Convert.ToInt32(reader[0]), reader[1] as string, reader[2] as string, reader[3] as string));
                }
            }
            connection.Close();
        }

        void Clean_data()
        {
            список_сотрудниковDataGrid.Items.Clear();
        }
        public Workers()
        {
            InitializeComponent();
            /*this.MinWidth= */ this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения " +
                $" where Приказ_зачисление is not null and  Контактные_данные.ID = Сведения.ID", connection);
            Add_col();
            Add_data(sql);
            birth.IsEnabled = settlement.IsEnabled = street.IsEnabled = house.IsEnabled = flat.IsEnabled = false;
            OK.Visibility = lab.Visibility = Visibility.Hidden;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void Список_сотрудниковDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (список_сотрудниковDataGrid.Items.Count > 0)
            {
                /*  info_block.Document.Blocks.Clear();
                  info_block.AppendText("Информация о сотруднике:\n");*/
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                fio.Text = row.fio;
                phone.Text = row.phone;
                email_address.Text = row.email;
                bet.IsEnabled = false;
                OK.Visibility = Visibility.Hidden;
                SqlCommand sql = new SqlCommand($"select Дата_рождения, Населенный_пункт, Улица, Номер_дома, Номер_квартиры,  Конструкторское_бюро, Подразделение, Должность, Форма_допуска, Ставка, Оклад from Список_сотрудников, Список_должностей, Штатное_расписание " +
                   $"where Список_сотрудников.ID = ID_сотрудника and Штатное_расписание.ID = ID_должности and Список_сотрудников.ID ={row.id}", connection);
                connection.Open();
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            DateTime date = Convert.ToDateTime(reader[0]);
                            birth.Text = date.ToShortDateString();
                        }
                        catch { birth.Text = ""; }
                        settlement.Text = reader[1] as string;
                        street.Text = reader[2] as string;
                        house.Text = reader[3] as string;
                        flat.Text = reader[4] as string;

                        des_dep.Text = reader[5] as string;
                        dep.Text = reader[6] as string;
                        post.Text = reader[7] as string;
                        access.Text = Convert.ToString(reader[8]);
                        bet.Text = Convert.ToString(reader[9]);
                        salary.Text = Convert.ToString(reader[10]);
                    }
                }
                connection.Close();


            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string search_bar = fio_search.Text + "%";
            SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения " +
             $" where Приказ_зачисление is not null and Сведения.ID = Контактные_данные.ID and Контактные_данные.ФИО like N'{search_bar}'", connection);
            Clean_data();
            Add_data(sql);
        }

        private void All_Click(object sender, RoutedEventArgs e)
        {
            fio_search.Text = "";
            SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения " +
             $" where Приказ_зачисление is not null and  Контактные_данные.ID = Сведения.ID", connection);
            Clean_data();
            Add_data(sql);
        }

        private void Change_cont_Click(object sender, RoutedEventArgs e)
        {
            if (список_сотрудниковDataGrid.Items.Count > 0 && список_сотрудниковDataGrid.SelectedIndex > -1)
            {
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];

                check = !check;
                bool check_fio = false;
                string[] name = Convert.ToString(fio.Text).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (name.Length == 3 || name.Length == 2) check_fio = true;
                else check_fio = false;

                if (check == false)
                {
                    if (Regex.IsMatch(phone.Text, @"(\+7)[0-9]{6}[0-9]{4}") && Regex.IsMatch(email_address.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") && Regex.IsMatch(fio.Text, @"^[а-яА-Я\s-]+$") && check_fio == true)
                    {
                        SqlCommand q1 = new SqlCommand($"update Контактные_данные set ФИО=N'{fio.Text}', Номер_телефона=N'{phone.Text}', Электронная_почта=N'{email_address.Text}' where ID={row.id}", connection);

                        connection.Open();
                        q1.ExecuteNonQuery();
                        connection.Close();

                        System.Windows.MessageBox.Show("Изменения внесены");
                        Show(check);

                        SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения"
                + $" where Приказ_зачисление is not null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
                        Clean_data();
                        Add_data(sql);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Поля заполнены неверно"); check = true; Show(check);
                    }
                }
                else
                {
                    check = true; Show(check);
                }
            }
        }

        private void Change_pers_Click(object sender, RoutedEventArgs e)
        {
            if (список_сотрудниковDataGrid.Items.Count > 0 && список_сотрудниковDataGrid.SelectedIndex > -1)
            {
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];

                check1 = !check1;


                if (check1 == false)
                {
                    DateTime d;
                    if (DateTime.TryParse(birth.Text, out d) && Regex.IsMatch(settlement.Text, @"^[а-яА-Я-\s,.]+$") && Regex.IsMatch(street.Text, @"^[а-яА-Я-\s,.]+$") && Regex.IsMatch(house.Text, @"^[0-9/-]+$"))
                    {
                        if (Regex.IsMatch(flat.Text, @"^[0-9]+$") || flat.Text == "")
                        {
                            string date = d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString();
                            SqlCommand q1 = new SqlCommand();
                            if (flat.Text == "")
                                q1.CommandText = $"update Список_сотрудников set Дата_рождения='{date}', Населенный_пункт=N'{settlement.Text}', Улица=N'{street.Text}', Номер_дома=N'{house.Text}' where ID={row.id}";
                            else
                                q1.CommandText = $"update Список_сотрудников set Дата_рождения='{date}', Населенный_пункт=N'{settlement.Text}', Улица=N'{street.Text}', Номер_дома=N'{house.Text}', Номер_квартиры={flat.Text} where ID={row.id}";
                            q1.Connection = connection;
                            connection.Open();
                            q1.ExecuteNonQuery();
                            connection.Close();

                            System.Windows.MessageBox.Show("Изменения внесены");
                            Show1(check1);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Поля заполнены неверно");
                            check1 = true; Show1(check1);
                        }
                    }
                   
                    else
                    {
                        System.Windows.MessageBox.Show("Поля заполнены неверно");
                        check1 = true; Show1(check1);
                    }
                }
                else
                {
                    check1 = true; Show1(check1);
                }
            }
        }

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            string fio, mail;
            try
            {
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                fio = row.fio as string;
                mail = row.email as string;
            }
            catch
            {
                fio = ""; mail = "";
            }

            Email em = new Email("Workers", fio, mail);
            em.Show();
            this.Hide();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                Documents documents = new Documents(row.id, row.fio, "Workers");
                documents.Show();
                this.Hide();
            }
            catch { }
        }

        double rate = 0.0;
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (список_сотрудниковDataGrid.Items.Count > 0 && список_сотрудниковDataGrid.SelectedIndex > -1)
            {

                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                SqlCommand q1 = new SqlCommand($"select ID_должности from Список_должностей where ID_сотрудника={row.id}", connection);
                
                connection.Open();
                int id_post = (int)q1.ExecuteScalar();
                connection.Close();
                
                if (Regex.IsMatch(bet.Text, @"^[0-9.,]+$"))
                {
                    if (Convert.ToDouble(bet.Text.Replace('.', ',')) > 1 || Convert.ToDouble(bet.Text.Replace('.', ',')) > rate || Convert.ToDouble(bet.Text.Replace('.', ',')) == 0)
                         System.Windows.MessageBox.Show("Ставка указана неверно");
                     else
                     {
                        double salar = Convert.ToDouble(salary.Text) / bet_old;
                        salar *= Convert.ToDouble(bet.Text.Replace('.', ','));
                        salary.Text = salar.ToString();
                        q1.CommandText = $"update Список_должностей set Ставка={bet.Text.Replace(',', '.')}, Оклад={salar} where ID_сотрудника={row.id}";
                        connection.Open();
                        q1.ExecuteNonQuery();
                        connection.Close();

                        rate -= Math.Round(Convert.ToDouble(bet.Text.Replace('.', ',')));//Convert.ToDouble(bet.Text.Replace('.', ',')), 2);
                        q1.CommandText = $"update Штатное_расписание set Количество_вакантных={rate.ToString().Replace(',', '.')} where ID={id_post}";
                        connection.Open();
                        q1.ExecuteNonQuery();
                        connection.Close();

                        bet.IsEnabled = false;
                        OK.Visibility = lab.Visibility = Visibility.Hidden;
                        labb.Content = "Ставка:";
                     }
                }
                else
                {
                    System.Windows.MessageBox.Show("Ставка указана неверно");
                }
            }

        }

        double bet_old = 0.0;
        private void Post_change_Click(object sender, RoutedEventArgs e)
        {

            if (список_сотрудниковDataGrid.Items.Count > 0 && список_сотрудниковDataGrid.SelectedIndex > -1)
            {

                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                string message = "Если необходимо изменть информацию полность, нажмите \"Да\".\nЕсли необходимо изменить ставку, нажмите \"Нет\"";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = System.Windows.MessageBox.Show(message, "", buttons, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Add_worker add_worker = new Add_worker(row.fio, row.id, "Workers");
                    add_worker.Show();
                    this.Hide();
                }

                else if (result == MessageBoxResult.No)
                {
                   // Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];
                    SqlCommand q1 = new SqlCommand($"select ID_должности from Список_должностей where ID_сотрудника={row.id}", connection);

                    connection.Open();
                    int id_post = (int)q1.ExecuteScalar();
                    connection.Close();

                    q1.CommandText = $"select Количество_вакантных from Штатное_расписание where ID={id_post}";
                    connection.Open();
                    rate = (double)q1.ExecuteScalar();
                    connection.Close();

                    bet_old = Convert.ToDouble(bet.Text.Replace('.', ','));
                    rate += bet_old;
                    if (rate > 1) lab.Content = "*ставка не выше 1";
                    else lab.Content = "*ставка не выше " + rate.ToString();
                    bet.IsEnabled = true;
                    OK.Visibility = lab.Visibility = Visibility.Visible;
                    
                    labb.Content = "*Ставка:";
                }
            }
          
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            if (список_сотрудниковDataGrid.Items.Count > 0)
            {
                Contacts row = (Contacts)список_сотрудниковDataGrid.SelectedItems[0];

                string name = row.fio;
                string message = "Удалить информацию о сотруднике " + name + "?";
                MessageBoxResult result = MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string path = "", number = "";
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            path = fbd.SelectedPath;
                            number = InputBox.Show();
                            if (number != string.Empty)
                            {
                                name = row.fio.ToString() + ", №" + number;
                                DirectoryInfo dir = new DirectoryInfo(path);
                                dir.CreateSubdirectory(name);
                                SqlCommand q1 = new SqlCommand($"select ID_должности from Список_должностей where ID_сотрудника={row.id}", connection);
                                connection.Open();
                                int post_id = (int)q1.ExecuteScalar();

                                q1.CommandText = $"select Количество_вакантных from Штатное_расписание  where ID={post_id}";

                                int count = (int)q1.ExecuteScalar();
                                count++;
                                q1.CommandText = $"update  Штатное_расписание set Количество_вакантных={count} where ID={post_id}";
                                q1.ExecuteNonQuery();

                                q1.CommandText = $"delete from Контактные_данные where ID={row.id}";
                                q1.ExecuteNonQuery();

                                connection.Close();
                                System.Windows.MessageBox.Show("Информация о сотруднике удалена из системы. Создана папка по уволенному сотруднику");
                                SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения " +
                                    $" where Приказ_зачисление is not null and  Контактные_данные.ID = Сведения.ID", connection);
                                Clean_data();
                                Add_data(sql);
                            }
                        }
                    }
                    catch { }
                }
            }

           
        }

        private void Doc_excel_Click(object sender, RoutedEventArgs e)
        {
            string filter = ListQuestion.Show("Личные данные сотрудников", "Информация о документах сотрудников");
            if (filter != string.Empty)
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook book = app.Workbooks.Add(); ;
                Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets.Item[1];

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel 2010|*.xlsx";
                string file = "";
                if (filter == "docs")
                {

                    sfd.FileName = "Список документов сотрудников " + DateTime.Today.ToShortDateString();
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        file = sfd.FileName;
                    }
                    try
                    {
                        sheet.Cells[1, 1] = "ФИО";
                        sheet.Cells[1, 2] = "Копия паспорта";
                        sheet.Cells[1, 3] = "Диплом";
                        sheet.Cells[1, 4] = "Трудовая книжка";
                        sheet.Cells[1, 5] = "Военный билет";
                        sheet.Cells[1, 6] = "Фото";
                        sheet.Cells[1, 7] = "Мед. справка";
                        sheet.Cells[1, 8] = "Флюорография";
                        sheet.Cells[1, 9] = "Ученая степень";
                        sheet.Cells[1, 10] = "Удостоверение ветерана";
                        sheet.Cells[1, 11] = "СНИЛС";
                        sheet.Cells[1, 12] = "ИНН";
                        sheet.Cells[1, 13] = "Заявление на прием";
                        sheet.Cells[1, 14] = "Анкета";
                        sheet.Cells[1, 15] = "Заявленмие о ЗП";
                        sheet.Cells[1, 16] = "Обязательства";
                        sheet.Cells[1, 17] = "Согласие";

                        int i = 2;

                        SqlCommand sql = new SqlCommand($"select ФИО, Копия_паспорта, Диплом, Трудовая_книжка, Военный_билет, Фото, Мед_справка, Итоги_фл, Документы_УчС, Удостоверение_ветерана, " +
                           $"СНИЛС, ИНН, Заявление_на_прием, Анкета, Заявление_о_зп, Обязательство, Согласие from Контактные_данные, Сведения " +
                           $"where Контактные_данные.ID=Сведения.ID and Приказ_зачисление is not null order by ФИО", connection);

                        connection.Open();

                        using (SqlDataReader reader = sql.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int j = 1; j <= 17; j++)
                                {
                                    if(j>1)
                                {
                                    if (reader[j - 1].ToString()=="True") sheet.Cells[i, j] = "Да";
                                    else sheet.Cells[i, j] = "Нет";
                                }
                                    else sheet.Cells[i, j] = reader[j-1].ToString();
                                }
                                i++;
                            }
                        }
                        connection.Close();

                        Excel.Range range1 = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 17]];
                        range1.EntireColumn.AutoFit();

                        Excel.Range range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[i, 17]];
                        range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        range.EntireRow.AutoFit();

                        book.SaveAs(file);
                        book.Close();
                        app.Quit();

                        System.Windows.MessageBox.Show("Список создан");
                    }
                    catch { System.Windows.MessageBox.Show("Список не может быть создан"); }
                }
                else if (filter == "pers")
                {
                    sfd.FileName = "Информация о сотрудниках " + DateTime.Today.ToShortDateString();
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        file = sfd.FileName;
                    }

                    try
                    {
                        sheet.Cells[1, 1] = "ФИО";
                        sheet.Cells[1, 2] = "Дата рождения";
                        sheet.Cells[1, 3] = "Номер телефона";
                        sheet.Cells[1, 4] = "Электронная почта ";
                        sheet.Cells[1, 5] = "Должность";
                        sheet.Cells[1, 6] = "Населенный пункт";
                        sheet.Cells[1, 7] = "Улица";
                        sheet.Cells[1, 8] = "№ дома";
                        sheet.Cells[1, 9] = "№ квартиры";
                        int i = 2;

                        SqlCommand sql = new SqlCommand($"select ФИО, Дата_рождения, Номер_телефона, Электронная_почта, Должность,  Населенный_пункт, Улица, Номер_дома, Номер_квартиры " +
                            $"from Контактные_данные, Список_должностей, Список_сотрудников, Штатное_расписание " +
                            $"where Контактные_данные.ID=Список_сотрудников.ID and ID_должности=Штатное_расписание.ID and Список_сотрудников.ID=ID_сотрудника order by ФИО", connection);

                        connection.Open();

                        using (SqlDataReader reader = sql.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int j = 1; j <= 9; j++)
                                {
                                    if (j == 2 && reader[j-1].ToString()!=string.Empty)
                                        sheet.Cells[i, j] = Convert.ToDateTime(reader[j - 1]).ToShortDateString();
                                    else sheet.Cells[i, j] = reader[j-1].ToString();
                                }
                                i++;
                            }
                        }
                        connection.Close();
                        Excel.Range range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[i, 7]];
                        range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        range.EntireColumn.AutoFit();
                        range.EntireRow.AutoFit();

                        book.SaveAs(file);
                        book.Close();
                        app.Quit();

                        System.Windows.MessageBox.Show("Список создан");
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Список не может быть создан");
                    }
                }
            }
        }
    }
}
