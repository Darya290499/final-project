using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Recruitment.xaml
    /// </summary>
    public partial class Recruitment : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
        void Show(bool check)
        {
            if(check==true)
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = true; ;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Visible;
                контактные_данныеDataGrid.Height = 191;
                //контактные_данныеDataGrid.Margin.Top = 114;
                контактные_данныеDataGrid.Margin = new Thickness(14,114,0,0);
            }
            else
            {
                fio.Focusable = phone.Focusable = email_address.Focusable = false;
                l2.Visibility = l3.Visibility = l4.Visibility = Visibility.Hidden;
                контактные_данныеDataGrid.Height = 241;
                //контактные_данныеDataGrid.Margin.Top = 114;
                контактные_данныеDataGrid.Margin = new Thickness(14, 64, 0, 0);
            }
        }

        bool check = false;
        public Recruitment()
        {
            InitializeComponent();
            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения"
                + $" where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
            Add_col();
            Add_data(sql);
        }
        
        

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

            for(int i = 1; i < 4; i++)
            {
                контактные_данныеDataGrid.Columns.Add(columns[i]);
            }

            columns[0].Binding = new Binding("id");
            columns[1].Binding = new Binding("fio");
            columns[2].Binding = new Binding("phone");
            columns[3].Binding = new Binding("email");
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
            Add_new add_new = new Add_new();
            add_new.Show();
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
             SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения "
                 + $"where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID and Контактные_данные.ФИО like N'{search_bar}'", connection);
            Clean_data();
            Add_data(sql);
        }
        
        private void All_Click(object sender, RoutedEventArgs e)
        {
            fio_search.Text = "";
            SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения" 
                + $" where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
            Clean_data();
            Add_data(sql);
        }

       

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            string fio, mail;
            try
            {
                Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];
                fio = row.fio as string;
                mail = row.email as string;
            }
            catch
            {
                fio = ""; mail = "";
            }

            Email em = new Email("Recruitment", fio, mail);
            em.Show();
            this.Hide();
        }

        

        private void Docs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Contacts row = (Contacts)контактные_данныеDataGrid.SelectedItems[0];
                Documents documents = new Documents(row.id, row.fio, "Recruitment");
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
                    
                        connection.Open();
                        q1.ExecuteNonQuery();
                        connection.Close();

                        MessageBox.Show("Изменения внесены");
                        Show(check);

                        SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения"
                + $" where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
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

                    SqlCommand sql = new SqlCommand($"select Контактные_данные.ID, Контактные_данные.ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения"
               + $" where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
                    Clean_data();
                    Add_data(sql);
                }
            }
        }

        private void Doc_excel_Click(object sender, RoutedEventArgs e)
        {
            string filter = ListQuestion.Show("Контактные данные кандидатов", "Информация о документах кандидатов");
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

                    sfd.FileName = "Список документов кандидатов " + DateTime.Today.ToShortDateString();
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
                           $"СНИЛС, ИНН, Заявление_на_прием, Анкета, Заявление_о_зп, Обязательство, Согласие from Контактные_данные, Сведения " 
                           + $"where Контактные_данные.ID=Сведения.ID and Приказ_зачисление is null order by ФИО", connection);

                        connection.Open();

                        using (SqlDataReader reader = sql.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int j = 1; j <= 17; j++)
                                {
                                    if (j > 1)
                                    {
                                        if (reader[j - 1].ToString() == "True") sheet.Cells[i, j] = "Да";
                                        else sheet.Cells[i, j] = "Нет";
                                    }
                                    else sheet.Cells[i, j] = reader[j - 1].ToString();
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
                    sfd.FileName = "Контактные данные кандидатов " + DateTime.Today.ToShortDateString();
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        file = sfd.FileName;
                    }

                    try
                    {
                        sheet.Cells[1, 1] = "ФИО";
                        sheet.Cells[1, 2] = "Номер телефона";
                        sheet.Cells[1, 3] = "Электронная почта ";
                        int i = 2;

                        SqlCommand sql = new SqlCommand($"select ФИО, Номер_телефона, Электронная_почта from Контактные_данные, Сведения " +
                            $"where Контактные_данные.ID=Сведения.ID and Приказ_зачисление is null order by ФИО", connection);

                        connection.Open();

                        using (SqlDataReader reader = sql.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int j = 1; j <= 3; j++)
                                {
                                    sheet.Cells[i, j] = reader[j - 1].ToString();
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
