using System;
using System.Collections.Generic;
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
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;
using System.IO;
using System.Data.SqlClient;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Email.xaml
    /// </summary>

    public partial class Email : Window
    {
        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\diplom; Initial Catalog = diplom; Integrated Security = True");
        List<string> data = new List<string>();
            
        public Email(string form)
        {
            InitializeComponent();

            if(form== "Recruitment")
            {
                SqlCommand sql = new SqlCommand($"select  ФИО, Электронная_почта from Контактные_данные",connection);

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
            else
            {
                SqlCommand sql = new SqlCommand($"select  ФИО, Электронная_почта from Список_сотрудников", connection);

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
            File.WriteAllText("Forms.txt", String.Empty);
            data.Clear();
            Recruitment recr = new Recruitment();
            recr.Show();
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

                }
            }
            catch { MessageBox.Show("Неверно заполнен адрес отправителя или адрес получателя или пароль"); }
            
        }
        

        private void File_Click(object sender, RoutedEventArgs e)
        {
            //paths.Clear();
            var dialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (string file in dialog.FileNames)
                {
                    //List<string> name=new List<string>();
                    paths.Add(file);
                    string[] name = file.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    attachs.Items.Add(name[name.Count()-1]);
                  //  attachs.SelectedIndex = attachs.Items.Count - 1;
                    //attachs.SelectedItem
                }
                
            }
        }

        
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete");
        }

        private void Fio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            address_to.Text = "";
            string[] strings = data[fio.SelectedIndex].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            address_to.Text = strings[1];
        }
    }
}
