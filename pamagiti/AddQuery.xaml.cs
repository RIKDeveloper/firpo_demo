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

        private User user { get; set; }
        private string type { get; set; }
        public AddQuery(string type, User user)
        {
            InitializeComponent();
            this.user = user;
            this.type = type;
            switch (this.type)
            {
                case "add":
                    date_create.SelectedDate = DateTime.Now;

                    break;
                case "change":
                    save.Content = "Применить";
                    status_block.Visibility = Visibility.Visible;
                    executor_block.Visibility = Visibility.Visible;
                    date_finish_block.Visibility = Visibility.Visible;
                    comment_block.Visibility = Visibility.Visible;
                    if (this.user.Id == 1)
                    {
                        date_finish.SelectedDate = DateTime.Now;
                        status.IsReadOnly = false;
                        executor.IsReadOnly = false;
                        date_finish.IsEnabled = true;
                    }
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
