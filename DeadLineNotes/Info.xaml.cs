using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeadLineNotes
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        private Info()
        {
            InitializeComponent();
        }

        private static Info self;
        public static Info GetInstance()
        {
            if (self == null) self = new Info();
            return self;
        }

        private void border_infoheader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void img_close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
    }
}
