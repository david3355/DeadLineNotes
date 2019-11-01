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
    /// Interaction logic for NoteEditor.xaml
    /// </summary>
    public partial class NoteEditor : Window
    {
        public NoteEditor(NoteStruct Note)
        {
            InitializeComponent();
            LoadNote(Note);
        }

        private NoteStruct ownNote;

        public NoteStruct OwnNote
        {
            get { return ownNote; }
        }

        private void LoadNote(NoteStruct Note)
        {
            for (int i = 0; i < 24; i++) combo_hours.Items.Add(i);
            for (int i = 0; i < 60; i++) combo_minutes.Items.Add(i);
            ownNote = Note;
            txt_note.Text = Note.Note;
            date_deadline.SelectedDate = Note.DeadLine;
            combo_hours.SelectedItem = Note.DeadLine.Hour;
            combo_minutes.SelectedItem = Note.DeadLine.Minute;
            if (!Note.HasDeadline) DisableDatePanel();
            txt_note.Focus();
            txt_note.SelectAll();
            check_do_notify.IsChecked = Note.DoNotify;
            check_do_notify.IsEnabled = MainWindow.notificationsEnabled;
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btn_save_note_Click(object sender, RoutedEventArgs e)
        {
            ownNote.ChangeNote(txt_note.Text);
            DateTime selected_date = date_deadline.SelectedDate.Value;
            DateTime date_to_set;
            if (stack_date.IsEnabled == true)
            {
                int h = 0;
                int m = 0;
                if (combo_hours.SelectedIndex != -1) h = Convert.ToInt32(combo_hours.SelectedItem);
                if (combo_minutes.SelectedIndex != -1) m = Convert.ToInt32(combo_minutes.SelectedItem);
                date_to_set = new DateTime(selected_date.Year, selected_date.Month, selected_date.Day, h, m, 0);
                ownNote.ChangeDeadLine(date_to_set);
            }
            else
            {
                ownNote.SetNoDeadLine();
            }
            ownNote.SetDoNotify(check_do_notify.IsChecked.Value);
            
            this.Close();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void img_date_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (img_date.Visibility == Visibility.Visible)
            {
                EnableDatePanel();
            }
            else
            {
                DisableDatePanel();
            }
        }

        private void EnableDatePanel()
        {
            img_date.Visibility = Visibility.Collapsed;
            img_no_date.Visibility = Visibility.Visible;
            stack_date.IsEnabled = true;
        }

        private void DisableDatePanel()
        {
            img_date.Visibility = Visibility.Visible;
            img_no_date.Visibility = Visibility.Collapsed;
            stack_date.IsEnabled = false;
        }
    }
}
