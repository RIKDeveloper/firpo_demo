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
        private class Status
        {
            public int Value { get; set; }
            public string Name { get; set; }

        }
        
        private class Query
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string OrgType { get; set; }
            public string Desc { get; set; }
            public int CustomerId {  get; set; }
            public string Customer { get; set; }
            public int ExecutorId { get; set; }
            public string Executor { get; set; }
            public Status Status { get; set; }
            public string Date { get; set; }
            public Status[] StatusSource { get; set; }
        }

        public DataGrid()
        {
            InitializeComponent();
            List<Query> queryList = new List<Query>
            {
                new Query { Id=1, Title="Печатная машинка Брауни", CustomerId=1, Customer="ООО Печатные машинки", ExecutorId=1, Executor="ООО Ремонт Печати", Desc="Описание заявки", Status=new Status { Value=1, Name="Новое" }, Date="10.10.23", StatusSource=new Status[] { new Status { Value=1, Name="Новое" } } },
            };
            queryGrid.ItemsSource = queryList;
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Add_Query(object sender, RoutedEventArgs e)
        {
            AddQuery addQuery = new AddQuery("add");
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
