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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private Settings(MainWindow MainProgram)
        {
            InitializeComponent();
            main = MainProgram;
            backup = NoteBackup.GetInstance();
            Init();
        }

        private NoteBackup backup;
        private MainWindow main;
        private Color bodyColor, headerColor;

        private static Settings self;
        public static Settings GetInstance(MainWindow MainProgram)
        {
            if (self == null) self = new Settings(MainProgram);
            return self;
        }

        private void Init()
        {
            LangInit();
            DateFormInit();
            //bodyColor = (Color)App.Current.TryFindResource("c_body");
            bodyColor = backup.GetSavedColor();
            headerColor = (Color)App.Current.TryFindResource("c_header");
            
            slide_a.Value = 255;
            slide_r.Value = bodyColor.R;
            slide_g.Value = bodyColor.G;
            slide_b.Value = bodyColor.B;
            SetColors();
            slide_a.ValueChanged += slide_ValueChanged;
            slide_r.ValueChanged += slide_ValueChanged;
            slide_g.ValueChanged += slide_ValueChanged;
            slide_b.ValueChanged += slide_ValueChanged;

            check_notifications_enabled.IsChecked = backup.GetNotificationsEnabled();
            check_notifications_enabled.Checked += check_notifications_enabled_Changed;
            check_notifications_enabled.Unchecked += check_notifications_enabled_Changed;
        }

        private void LangInit()
        {
            combo_langs.Items.Add(new LangStruct("English", "img/flags/uk_flag.png", new Uri("res/English.xaml", UriKind.Relative)));
            combo_langs.Items.Add(new LangStruct("Magyar", "img/flags/hun_flag.png", new Uri("res/Hungarian.xaml", UriKind.Relative)));
            combo_langs.SelectedIndex = 0;
        }

        private void DateFormInit()
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < DateFormat.NumberOfDateForms; i++)
            {
                string d = now.ToString(DateFormat.GetDateForm(i));
                combo_dateformats.Items.Add(d);
            }
            combo_dateformats.SelectedIndex = 0;
        }

        private void img_close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        private void border_settingsheader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void check_topmost_Change(object sender, RoutedEventArgs e)
        {
            if (check_topmost.IsChecked != null)
                main.Topmost = check_topmost.IsChecked.Value;
        }

        private void combo_langs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Current.Resources.MergedDictionaries[0].Source = (combo_langs.SelectedItem as LangStruct).ResourceUri;
            main.list_todos.Items.Refresh();
        }

        private void SetColors()
        {
            bodyColor.A = (byte)slide_a.Value;
            bodyColor.R = (byte)slide_r.Value;
            bodyColor.G = (byte)slide_g.Value;
            bodyColor.B = (byte)slide_b.Value;
            byte x = 30;
            headerColor.R = Darken(bodyColor.R, x);
            headerColor.G = Darken(bodyColor.G, x);
            headerColor.B = Darken(bodyColor.B, x);
            App.Current.Resources["c_body"] = bodyColor;
            App.Current.Resources["c_header"] = headerColor;
        }

        private void slide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetColors();
        }

        private byte Darken(byte Color, int Value)
        {
            int v = Color - Value;
            if (v < 0) return 0;
            return (byte)v;
        }

        private void combo_dateformats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Current.Resources["s_dateform"] = DateFormat.GetDateForm(combo_dateformats.SelectedIndex);
            main.RefreshItemsView();
        }

        private void btn_save_color_Click(object sender, RoutedEventArgs e)
        {
            backup.SaveNoteColors((int)slide_a.Value, (int)slide_r.Value, (int)slide_g.Value, (int)slide_b.Value);
        }

        private void btn_load_orig_color_Click(object sender, RoutedEventArgs e)
        {

        }

        private void check_notifications_enabled_Changed(object sender, RoutedEventArgs e)
        {
            if (check_notifications_enabled.IsChecked != null)
            {
                MainWindow.notificationsEnabled = check_notifications_enabled.IsChecked.Value;
                backup.SaveNotificationsEnabled(check_notifications_enabled.IsChecked.Value);
            }
        }
    }
}
