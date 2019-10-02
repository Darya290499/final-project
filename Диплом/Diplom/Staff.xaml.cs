using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Media;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Staff.xaml
    /// </summary>
    public partial class Staff : System.Windows.Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);

       // string post_state = "";


        void Add_col(int count)
        {
            DataGridTextColumn []columns = new DataGridTextColumn[8];
            for(int i=0;i<8;i++)
            {
                columns[i] = new DataGridTextColumn();
            }

            columns[0].Header = "ID";
            columns[1].Header = "Конструкторское\nбюро";
            columns[2].Header = "Подразделение";
            columns[3].Header = "Должность";
            columns[4].Header = "Количество ";
            columns[5].Header = "Вакантные";
            columns[6].Header = "Оклад";
            columns[7].Header = "Допуск";
            

            for (int i = 0; i < 7; i++) columns[i].IsReadOnly = true;
              
               
            
            columns[0].Binding = new System.Windows.Data.Binding("id");
            columns[1].Binding = new System.Windows.Data.Binding("des_dep");
            columns[2].Binding = new System.Windows.Data.Binding("dep");
            columns[3].Binding = new System.Windows.Data.Binding("post");
            columns[4].Binding = new System.Windows.Data.Binding("count");
            columns[5].Binding = new System.Windows.Data.Binding("vacant");
            columns[6].Binding = new System.Windows.Data.Binding("all_salary");
            columns[7].Binding = new System.Windows.Data.Binding("access");
            

            if(count ==6)
                for(int i=1;i<8;i++)
                    штатное_расписаниеDataGrid.Columns.Add(columns[i]);
            else if(count==5)
            
                for (int i = 2; i < 8; i++)
                    штатное_расписаниеDataGrid.Columns.Add(columns[i]);
            else
                for (int i = 3; i < 8; i++)
                    штатное_расписаниеDataGrid.Columns.Add(columns[i]);
            
        }

        void Add_data(SqlCommand sql)
        {
            connection.Open();

            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                { 
                        штатное_расписаниеDataGrid.Items.Add(new StaffList(Convert.ToInt32(reader[0]),reader[1] as string, reader[2] as string, reader[3] as string, Convert.ToInt32(reader[4]), Convert.ToDouble(reader[5]), reader[6] as string, Convert.ToInt32(reader[7])));
                }
            }
            connection.Close();
        }

        void Clean_grid()
        {
            штатное_расписаниеDataGrid.Items.Clear();
            штатное_расписаниеDataGrid.Columns.Clear();
        }
        
        void Clean()
        {
            des_dep.Text = dep.Text = post.Text = count.Text = salary.Text = access.Text = "";
        }
        int Show(int identif)
        {
            if (identif == 1)
            {
                /* DoubleAnimation form = new DoubleAnimation();
                 form.From = this.ActualWidth;
                 form.To = 1325;
                 form.Duration = TimeSpan.FromSeconds(10);
                 this.BeginAnimation(Button.WidthProperty, form);*/

                this.Width = this.MinWidth = this.MaxWidth = 1325;
                dep.IsEnabled = des_dep.IsEnabled = true;
                return 1;
            }
            else if (identif == 2)
            {
                this.Width = this.MinWidth = this.MaxWidth = 1325;
                dep.IsEnabled = des_dep.IsEnabled = false;
                return 2;
            }
            else
            {
                this.Width = this.MinWidth = this.MaxWidth = 1042;
                return 0;
            }
            
        }

        public Staff()
        {
            InitializeComponent();
            /*this.MinWidth = */this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            des_dep.IsEnabled = dep.IsEnabled = post.IsEnabled = count.IsEnabled = salary.IsEnabled = access.IsEnabled = false;
            ok.Visibility = Visibility.Hidden;

            department.IsEnabled = false;

            SqlCommand sql = new SqlCommand($"select* from Штатное_расписание order by Конструкторское_бюро",connection);
            Add_col(6);
            Add_data(sql);
            if (штатное_расписаниеDataGrid.Items.Count > 0) штатное_расписаниеDataGrid.SelectedIndex = 0;

            sql.CommandText = $"select distinct Конструкторское_бюро from Штатное_расписание";
            connection.Open();
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    design_department.Items.Add(reader[0] as string);
                }
            }
            connection.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void Design_department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            department.Items.Clear();

            if (design_department.SelectedIndex == -1) department.IsEnabled = false;
            else department.IsEnabled = true;
            

            SqlCommand sql = new SqlCommand($"select distinct Подразделение from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}'", connection);
            //sql.CommandText = $"select distinct Подразделение from Штатное_расписание";
            connection.Open();
            using (SqlDataReader reader = sql.ExecuteReader())
            {
                while (reader.Read())
                {
                    department.Items.Add(reader[0] as string);
                }
            }
            connection.Close();

            department.SelectedIndex = -1;
            Clean_grid();
            string v = "";
            if (filter == "vacant") v = " and Количество_вакантных>0";
            sql.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}'" + v + " order by Подразделение";
            Add_col(5);
            Add_data(sql);
        }
        
        private void Department_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (department.SelectedIndex != -1)
            {
                string v = "";
                if (filter == "vacant") v = " and Количество_вакантных>0";
                SqlCommand sql = new SqlCommand($"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}'"+v+" order by Должность", connection);
                Clean_grid();
                Add_col(4);
                Add_data(sql);
            }
        }

        string filter = "";
        private void All_list_Click(object sender, RoutedEventArgs e)
        {
            vacants.Background = new SolidColorBrush(Color.FromArgb(100,205,205,205));
            //post_state = "";
            //department.IsEnabled = false;
            department.Items.Clear();
            design_department.SelectedIndex = -1;

            SqlCommand sql = new SqlCommand($"select* from Штатное_расписание order by Конструкторское_бюро", connection);
            Clean_grid();
            Add_col(6);
            Add_data(sql);
            filter = "";
        }

        private void Vacant_Click(object sender, RoutedEventArgs e)
        {
            // post_state = 
            filter ="vacant";
            vacants.Background = new SolidColorBrush(Colors.LightGray);
            SqlCommand sql = new SqlCommand($"select count(*) from Штатное_расписание where Количество_вакантных > 0", connection);
            connection.Open();
            int count = (int)sql.ExecuteScalar();
            connection.Close();
            if (count > 0)
            {
                if (design_department.SelectedIndex != -1 && department.SelectedIndex != -1)
                {
                    sql.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}' and Количество_вакантных>0 order by Должность";
                    Clean_grid();
                    Add_col(4);

                    Add_data(sql);
                }
                else if (design_department.SelectedIndex != -1 && department.SelectedIndex == -1)
                {
                    sql.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}'and Количество_вакантных>0 order by Подразделение";
                    Clean_grid();
                    Add_col(5);
                    Add_data(sql);
                }
                else
                {
                    sql.CommandText = $"select* from Штатное_расписание where Количество_вакантных>0  order by Конструкторское_бюро";
                    Clean_grid();
                    Add_col(6);
                    Add_data(sql);

                }
            }
            else System.Windows.MessageBox.Show("Нет вакантных должностей");
        }

        private void Add_post_Click(object sender, RoutedEventArgs e)
        {
           Add_post ap = new Add_post();
            ap.Show();
            this.Hide();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            des_dep.IsEnabled = dep.IsEnabled = post.IsEnabled = count.IsEnabled = salary.IsEnabled = access.IsEnabled = true;
            ok.Visibility = Visibility.Visible;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (штатное_расписаниеDataGrid.Items.Count > 0 && штатное_расписаниеDataGrid.SelectedIndex > -1)
            {

                StaffList row = (StaffList)штатное_расписаниеDataGrid.SelectedItems[0];
                string message = "Удалить всю информацию о выбранной должности?";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = System.Windows.MessageBox.Show(message, "", buttons, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SqlCommand q1 = new SqlCommand($"delete from Штатное_расписание where ID={row.id}", connection);
                    connection.Open();
                    q1.ExecuteNonQuery();
                    connection.Close();

                    System.Windows.MessageBox.Show("Информация удалена");

                    string v = "";
                    if (filter == "vacant") v = " and Количество_вакантных>0 ";
                    if (design_department.SelectedIndex != -1 && department.SelectedIndex != -1)
                    {

                        Clean_grid();

                        q1.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}'" + v + " order by Должность";
                        Add_col(4);

                        Add_data(q1);
                    }
                    else if (design_department.SelectedIndex != -1 && department.SelectedIndex == -1)
                    {
                        Clean_grid();
                        q1.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}'" + v + " order by Подразделение";
                        Add_col(5);
                        Add_data(q1);
                    }
                    else
                    {
                        //
                        if ((filter == "vacant")) v = " where" + v;
                        Clean_grid();
                        q1.CommandText = $"select* from Штатное_расписание" + v + " order by Конструкторское_бюро";
                        Add_col(6);
                        Add_data(q1);
                    }
                }

                
            }
        }

        private void Штатное_расписаниеDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (штатное_расписаниеDataGrid.Items.Count > 0 && штатное_расписаниеDataGrid.SelectedIndex > -1)
                {

                    StaffList row = (StaffList)штатное_расписаниеDataGrid.SelectedItems[0];
                    des_dep.Text = row.des_dep;
                    dep.Text = row.dep;
                    post.Text = row.post;
                    count.Text = row.count.ToString();
                    salary.Text = row.all_salary;
                    access.Text = row.access.ToString();
                }
            }
            catch { }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (штатное_расписаниеDataGrid.Items.Count > 0 && штатное_расписаниеDataGrid.SelectedIndex > -1)
            {
                StaffList row = (StaffList)штатное_расписаниеDataGrid.SelectedItems[0];
                double va = row.vacant;
                int c = row.count;
                try
                {
                    c = Convert.ToInt32(count.Text) - c;
                    va += c;
                    SqlCommand sql = new SqlCommand($"update Штатное_расписание set Конструкторское_бюро=N'{des_dep.Text}', Подразделение=N'{dep.Text}', Должность=N'{post.Text}'," +
                        $"Количество_должностей={count.Text}, Зарплата_полная_ставка=N'{salary.Text}', Количество_вакантных={va.ToString().Replace(',', '.')}, Форма_допуска={access.Text} where ID={row.id}", connection);

                    connection.Open();
                    sql.ExecuteNonQuery();
                    connection.Close();
                    System.Windows.MessageBox.Show("Данные изменены");
                    string v = "";
                    if (filter == "vacant") v = " and Количество_вакантных>0 ";
                    if (design_department.SelectedIndex != -1 && department.SelectedIndex != -1)
                    {

                        Clean_grid();

                        sql.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}'"+v+" order by Должность";
                        Add_col(4);

                        Add_data(sql);
                    }
                    else if (design_department.SelectedIndex != -1 && department.SelectedIndex == -1)
                    {
                        Clean_grid();
                        sql.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}'"+v+" order by Подразделение";
                        Add_col(5);
                        Add_data(sql);
                    }
                    else
                    {
                       //
                       if((filter == "vacant")) v = " where" + v;
                        Clean_grid();
                        sql.CommandText = $"select* from Штатное_расписание" + v + " order by Конструкторское_бюро";
                        Add_col(6);
                        Add_data(sql);
                    }
                    Clean();
                    des_dep.IsEnabled = dep.IsEnabled = post.IsEnabled = count.IsEnabled = salary.IsEnabled = access.IsEnabled = false;
                    ok.Visibility = Visibility.Hidden;
                }
                catch { System.Windows.MessageBox.Show("Некорректное заполнение полей"); }
            }
        }

        class Filters
        {
            public SqlCommand q;
            public int step;
            public string file_name;
            public int column_width;
        };

        Filters GetFilters()
        {
            Filters obj = new Filters();
            obj.q = new SqlCommand($"", connection);
            obj.step = 0;
            obj.file_name = "";
            obj.column_width = 0;
           
            if (filter == "vacant")
            {
                if (design_department.SelectedIndex != -1 && department.SelectedIndex == -1)
                {
                    obj.file_name = "Вакантные должности в " + design_department.SelectedItem + " за ";
                    obj.q.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Количество_вакантных>0 order by Подразделение";
                    obj.step = 1;
                    obj.column_width = 0;
                }
                else if (design_department.SelectedIndex != -1 && department.SelectedIndex != -1)
                {
                    obj.file_name = "Вакантные должности в " + design_department.SelectedItem + ", подразделение " + department.SelectedItem + "_за_";
                    obj.q.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}' and Количество_вакантных>0 order by Должность";
                    obj.step = 2;
                    obj.column_width = 0;
                }
                else
                {
                    obj.file_name = "Вакантные должности за ";
                    obj.q.CommandText = $"select* from Штатное_расписание where Количество_вакантных>0 order by Конструкторское_бюро";
                    obj.step = 0;
                    obj.column_width = 17;
                }
            }
            else
            {
                if (design_department.SelectedIndex != -1 && department.SelectedIndex == -1)
                {
                    obj.file_name = "Штатная расстановка в " + design_department.SelectedItem + " за ";
                    obj.q.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' order by Подразделение";
                    obj.step = 1;
                    obj.column_width = 0;
                }
                else if (design_department.SelectedIndex != -1 && department.SelectedIndex != -1)
                {
                    obj.file_name = "Штатная расстановка в " + design_department.SelectedItem + ", подразделение " + department.SelectedItem + " за ";
                    obj.q.CommandText = $"select* from Штатное_расписание where Конструкторское_бюро=N'{design_department.SelectedItem}' and Подразделение=N'{department.SelectedItem}' order by Должность";
                    obj.step = 2;
                    obj.column_width = 0;
                }
            }
            return obj;
        }
            
    
        private void Doc_excel_Click(object sender, RoutedEventArgs e)
        {
            if (штатное_расписаниеDataGrid.Items.Count > 0)
            {
                SqlCommand sql= new SqlCommand($"", connection); ; string file_name = ""; string file = ""; int step = 0;
                int width = 0;
                Excel.Application app = new Excel.Application();
                Excel.Workbook book = app.Workbooks.Add(); ;
                Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets.Item[1];

                if (design_department.SelectedIndex > -1 || design_department.SelectedIndex > -1 || filter == "vacant")
                {
                    string message1 = "Учитывать фильтры при построении списка?";
                    MessageBoxButton buttons1 = MessageBoxButton.YesNo;
                    MessageBoxResult result1 = System.Windows.MessageBox.Show(message1, "", buttons1, MessageBoxImage.Question);
                    if (result1 == MessageBoxResult.Yes)
                    {
                        Filters filters = new Filters();
                        filters = GetFilters();
                        sql = filters.q;
                        file_name = filters.file_name;
                        step = filters.step;
                        width = filters.column_width;

                        for (int j = 0; j < штатное_расписаниеDataGrid.Columns.Count; j++)
                            sheet.Cells[1, j + 1] = штатное_расписаниеDataGrid.Columns[j].Header;
                    }
                    else
                    {
                        file_name = "Штатная расстановка за ";
                        sql.CommandText = $"select* from Штатное_расписание order by Конструкторское_бюро";
                        width = 0;
                        sheet.Cells[1, 1] = "Конструкторское\nбюро";
                        sheet.Cells[1, 2] = "Подразделение";
                        sheet.Cells[1, 3] = "Должность";
                        sheet.Cells[1, 4] = "Количество ";
                        sheet.Cells[1, 5] = "Вакантные";
                        sheet.Cells[1, 6] = "Оклад";
                        sheet.Cells[1, 7] = "Допуск";
                    }
                }
                else
                {
                    file_name = "Штатная расстановка за ";
                    sql.CommandText = $"select* from Штатное_расписание order by Конструкторское_бюро";
                    width = 0;

                    for (int j = 0; j < штатное_расписаниеDataGrid.Columns.Count; j++)
                        sheet.Cells[1, j + 1] = штатное_расписаниеDataGrid.Columns[j].Header;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = file_name+DateTime.Today.ToShortDateString();
                sfd.Filter = "Excel 2010|*.xlsx";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    file = sfd.FileName;
                   try
                    {
                        
                        int i = 2;
                        connection.Open();

                        using (SqlDataReader reader = sql.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // sheet.Cells[i + 2, j]=reader//department.Items.Add(reader[0] as string);
                                for (int j = 1; j <= штатное_расписаниеDataGrid.Columns.Count; j++)
                                {
                                    sheet.Cells[i, j] = reader[j + step].ToString();
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

                        if(width!=0)
                            sheet.Columns[1].ColumnWidth = 17;
                        book.SaveAs(file);
                        book.Close();
                        app.Quit();

                        System.Windows.MessageBox.Show("Список создан");
                    }
                    catch { System.Windows.MessageBox.Show("Список не может быть создан"); }

                }


            }
            else System.Windows.MessageBox.Show("Список должностей пуст");
        }

    }
}


