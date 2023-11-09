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
    /// Логика взаимодействия для AddQuery.xaml
    /// </summary>
    public partial class AddQuery : Window
    {
        public AddQuery(string type)
        {
            InitializeComponent();
            switch (type)
            {
                case "add":
                    date_create.SelectedDate = DateTime.Now;
                    date_create.IsEnabled = false;
                    executor_block.Visibility = Visibility.Collapsed;
                    date_finish_block.Visibility = Visibility.Collapsed;
                    break;
                case "change":
                    save.Content = "Применить";
                    break;
            }
        }

        private void MenuItem_Click_Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Extensions.Exit_User();
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Extensions.Close_App();
        }
    }
}
