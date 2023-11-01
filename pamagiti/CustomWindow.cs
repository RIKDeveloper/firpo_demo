using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace pamagiti
{
    public partial class CustomWindow: Window
    {
       
        public void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
            login.Show();
        }

        public void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
