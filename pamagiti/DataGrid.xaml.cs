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

        public DataGrid(User user)
        {
            InitializeComponent();
            List<Query> queryList = BD.Get_Queries(user);
            queryGrid.ItemsSource = queryList;
            this.user = user;
            BD.Create_Connection(mc);
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Add_Query(object sender, RoutedEventArgs e)
        {
            AddQuery addQuery = new AddQuery("add", user);
            addQuery.Owner = this;
            addQuery.Show();
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Extensions.Close_App();
        }

        private void queryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
