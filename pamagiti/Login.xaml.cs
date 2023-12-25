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
            BD.Check_Connection(this);
            AddUser addUser = new AddUser();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User user = BD.Login_User(login.Text, password.Password);
            List<User> users = BD.Get_Users();
            if (user.Login == login.Text)
            {
                DataGrid dataGrid = new DataGrid(user);
                dataGrid.Show();
                this.Close();
            } else
            {
                MessageBox.Show("Пользователь не найден! Проверьте логин и пароль.");
            }
        }
    }
}
