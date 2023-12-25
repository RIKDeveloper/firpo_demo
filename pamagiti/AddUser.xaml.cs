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

namespace pamagiti
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
            role.ItemsSource = BD.Roles.Where(x=>x.Id!=4).Select(x => x.Name).ToList();
            role.SelectedIndex = 0;
        }
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Extensions.Close_App();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (password.Password != dPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            User u = new User();
            u.Surname = surname.Text;
            u.Name = name.Text;
            u.Patronomic = patronomic.Text;
            u.Email = email.Text;
            u.Phone = phone.Text;
            u.Password = password.Password;
            u.Role = BD.Roles[role.SelectedIndex];
            u.Login = login.Text;
            try
            {
                BD.AddUser(u);
                this.Close();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
