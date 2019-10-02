using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Configuration;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Email.xaml
    /// </summary>

    public partial class Email : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
        List<string> data = new List<string>();
        string from = "";    
        public Email(string form, string name, string mail)
        {
            InitializeComponent();

            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            from = form;
            if(form== "Recruitment" && name!="" && mail!="" )
            {
                SqlCommand sql = new SqlCommand($"select Контактные_данные.ФИО, Электронная_почта from Контактные_данные, Сведения where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by Контактные_данные.ФИО",connection);

                connection.Open();
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader[0] as string + "; " + reader[1] as string);
                        fio.Items.Add(reader[0] as string);
                    }
                }
                connection.Close();

                for(int i=0;i<data.Count;i++)
                {
                    string str = name + "; " + mail;
                    if (str == data[i]) fio.SelectedIndex = i;
                }
                    
            }
            else if(form=="Recruitment" && mail=="" && name=="")
            {
                SqlCommand sql = new SqlCommand($"select ФИО, Электронная_почта from Контактные_данные, Сведения where Приказ_зачисление is null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);
                connection.Open();
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader[0] as string + "; " + reader[1] as string);
                        fio.Items.Add(reader[0] as string);
                    }
                }
                connection.Close();
            }
            else if(form=="Workers" && mail!="" && name!="")
            {
                SqlCommand sql = new SqlCommand($"select ФИО, Электронная_почта from Контактные_данные, Сведения where Приказ_зачисление is not null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);

                connection.Open();
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader[0] as string + "; " + reader[1] as string);
                        fio.Items.Add(reader[0] as string);
                    }
                }
                connection.Close();

                for (int i = 0; i < data.Count; i++)
                {
                    string str = name + "; " + mail;
                    if (str == data[i]) fio.SelectedIndex = i;
                }

            }
            else if (form == "Workers" && mail == "" && name == "")
            {
                SqlCommand sql = new SqlCommand($"select ФИО, Электронная_почта from Контактные_данные, Сведения where Приказ_зачисление is not null and Сведения.ID = Контактные_данные.ID order by ФИО", connection);

                connection.Open();
                using (SqlDataReader reader = sql.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader[0] as string + "; " + reader[1] as string);
                        fio.Items.Add(reader[0] as string);
                    }
                }
                connection.Close();
            }
        }

        List<string> paths = new List<string>();

        private void Window_Closed(object sender, EventArgs e)
        {
            data.Clear();
            if (from == "Recruitment")
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

        private void Send_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                MailAddress from = new MailAddress(address_from.Text);
                MailAddress to = new MailAddress(address_to.Text);

                using (MailMessage mail = new MailMessage(from, to))
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    mail.Subject = theme.Text;
                    mail.Body = text_body.Text;
                    for(int i = 0; i < paths.Count; i++)
                    {
                        mail.Attachments.Add(new Attachment(paths[i]));
                    }
                    string[] address = address_from.Text.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                    smtpClient.Host = "smtp." + address[1];
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(from.Address, "132around");

                    smtpClient.Send(mail);
                    MessageBox.Show("Письмо отправлено");

                    paths.Clear();
                    attachs.Items.Clear();
                    address_to.Clear();
                    fio.SelectedIndex = -1;
                    text_body.Clear();
                    theme.Clear();
                    this.Height = MinHeight = MaxHeight = 468;
                }
        }
         catch { MessageBox.Show("Неверно заполнен адрес отправителя или адрес получателя или пароль"); }

    }


    private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (string file in dialog.FileNames)
                {
                    paths.Add(file);
                    string[] name = file.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    attachs.Items.Add(name[name.Count()-1]);
                }
                if(paths.Count==0)
                    this.Height = MinHeight = MaxHeight = 468;
                else this.Height = MinHeight = MaxHeight = 591;
            }
        }
        
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            paths.RemoveAt(attachs.SelectedIndex);
            attachs.Items.RemoveAt(attachs.SelectedIndex);
            if (paths.Count == 0)
                this.Height = MinHeight = MaxHeight = 468;
            else
                this.Height = MinHeight = MaxHeight = 630;
            
        }

        private void Fio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            address_to.Text = ""; string[] strings;
            if (fio.SelectedIndex != -1)
            {
                strings = data[fio.SelectedIndex].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                address_to.Text = strings[1];
            }
            else address_to.Text = "";
        }
    }
}
