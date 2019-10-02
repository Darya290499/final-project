using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();

            this.MinWidth = this.MaxWidth = this.Width;
            this.MinHeight = this.MaxHeight = this.Height;

            
            ProcessStartInfo psi;
            psi = new ProcessStartInfo("cmd.exe", @"/c sqllocaldb start diplom");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
        

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);


            SqlCommand sql = new SqlCommand($"select count(*) from Пользователи where Имя_пользователя='{login.Text}' and Пароль='{password.Password}'",connection);
            int check = 0;
            connection.Open();
            check = (int)sql.ExecuteScalar();
            connection.Close();
            if (check!=0)
            {
                MainWindow window = new MainWindow();
                window.Show();
                this.Hide();
            }
            else MessageBox.Show("Неправильно указано имя пользователя или пароль");
        }
    }
}
