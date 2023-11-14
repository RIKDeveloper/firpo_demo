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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pamagiti
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            login.Text = "1";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int role = int.Parse(login.Text);
            User user = BD.Login_User(login.Text, password.Password);
            List<User> users = BD.Get_Users();
            if (users.Count < 0 )
            {
                user.Surname = "Иванов";
                user.Name = "Кирилл";
                user.Patronomic = "Романович";
                user.Email = "123@123.ru";
                user.Phone = "+79999999999";
                user.Role = new Role();
                user.Role.Id = 4;
                user.Login = "kikrdev";
                user.Password = "pamagiti";
                BD.Set_User(user);
            }
            if (user != null)
            {
                DataGrid dataGrid = new DataGrid(user);
                dataGrid.Show();
                this.Close();
            }
        }
    }
}
