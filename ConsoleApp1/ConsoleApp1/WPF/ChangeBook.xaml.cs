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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseChecker.WPF {
    /// <summary>
    /// Interaction logic for ChangeBook.xaml
    /// </summary>
    public partial class ChangeBook : Window {
        public ChangeBook() {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}