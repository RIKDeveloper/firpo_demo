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
    public static class Extensions
    { 
       
        public static void Exit_User()
        {
            Login login = new Login();
            login.Show();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() != typeof(Login))
                    window.Close();
            }
            
        }

        public static void Close_App()
        {
            Environment.Exit(0);
        }
    }
}
