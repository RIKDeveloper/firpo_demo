using System;
using System.CodeDom.Compiler;
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
    /// Логика взаимодействия для DataGrid.xaml
    /// </summary>
    public partial class DataGrid : Window
    {
        private User user { get; set; }
        private List<Query> queries {  get; set; }
        public DataGrid(User user)
        {
            InitializeComponent();
            List<Query> queryList = BD.Get_Queries(user);
            this.queries = queryList;
            queryGrid.ItemsSource = queryList;
            this.user = user;
            if (user.Role.Id == 3 || user.Role.Id == 4)
            {
                addUser.Visibility = Visibility.Visible;
            }
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Add_Query(object sender, RoutedEventArgs e)
        {
            AddQuery addQuery = new AddQuery("add", user, null);
            addQuery.Owner = this;
            addQuery.ShowDialog();
            List<Query> queryList = BD.Get_Queries(user);
            this.queries = queryList;
            queryGrid.ItemsSource = queryList;
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Extensions.Close_App();
        }

        private void queryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (user.Role.Id == 0)
            {
                MessageBox.Show("Нет прав для измения заявки.");
            } else
            {
                Query q = queryGrid.SelectedItem as Query;
                AddQuery addQuery = new AddQuery("change", user, q);
                addQuery.Owner = this;
                addQuery.ShowDialog();
                List<Query> queryList = BD.Get_Queries(user);
                this.queries = queryList;
                queryGrid.ItemsSource = queryList;
            }
            
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.Owner = this;
            addUser.ShowDialog();
        }
    }
}
