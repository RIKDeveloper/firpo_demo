﻿using System;
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
    public partial class AddQuery : CustomWindow
    {
        public AddQuery()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Back(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
