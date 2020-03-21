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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;

namespace DeadLineNotes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private DispatcherTimer inspector;
        private MediaPlayer player;
        private NoteBackup backup;
        private Settings settings;
        private Info info;

        private bool sorted;
        public static bool notificationsEnabled;
        private BitmapImage img_order_desc, img_order_asc;
        private ListSortDirection lastOrder;
    
        private static TimeSpan check_interval;
        public static TimeSpan CheckInterval { get { return check_interval; } }

        private void Init()
        {
            check_interval = new TimeSpan(0, 0, 30);
            settings = Settings.GetInstance(this);
            info = Info.GetInstance();
            sorted = false;
            lastOrder = ListSortDirection.Descending;
            img_order_desc = new BitmapImage(new Uri("img/down.png", UriKind.Relative));
            img_order_asc = new BitmapImage(new Uri("img/up.png", UriKind.Relative));
            player = new MediaPlayer();
            Uri u = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"sound/bell.mp3", UriKind.Absolute);
            if (!File.Exists(u.LocalPath)) MessageBox.Show(String.Format(Res("s_soundnf"), u.LocalPath));
            try
            {
                player.Open(u);
                player.MediaEnded += delegate(object s, EventArgs e) { player.Stop(); };
                player.Volume = 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Res("s_cannotopenf"), u.LocalPath, e.Message), Res("s_error"));
                player = null;
            }
            backup = NoteBackup.GetInstance();
            notificationsEnabled = backup.GetNotificationsEnabled();
            List<NoteStruct> backups = backup.ReadSavedNotes();
            foreach (NoteStruct note in backups) list_todos.Items.Add(note);
            SetWindowHeight();
            inspector = new DispatcherTimer();
            inspector.Interval = check_interval;
            inspector.Tick += Inspect;
            inspector.Start();
            SwitchOrder();
        }

        private void SetWindowHeight()
        {
            int itemNum = list_todos.Items.Count;
            if (itemNum > 10) itemNum = 10;
            int notehgt = 31;
            int space_bw_notes = 4;
            int window_header = 30;
            int listmargin = 2;
            this.Height = window_header + listmargin + itemNum * (notehgt + space_bw_notes) + 6;

            //this.Width = list_todos.ActualWidth + list_todos.Margin.Left + list_todos.Margin.Right;
        }

        public static string Res(string ResourceIdentifier)
        {
            return App.Current.TryFindResource(ResourceIdentifier).ToString();
        }

        void Inspect(object sender, EventArgs e)
        {
            lock (list_todos.Items)
            {
                foreach (NoteStruct note in list_todos.Items)
                {
                    note.CheckPriority();
                    if (note.TimeToWarn && note.DoNotify && notificationsEnabled)
                    {
                        NoteWarn nw = new NoteWarn(note);
                        nw.Show();
                        if (player != null) player.Play();
                        note.SetWarn();
                    }
                }
                if (sorted) Sort(lastOrder);
                else RefreshItemsView();
            }
        }

        private void AddNewNote()
        {
            NoteStruct n = new NoteStruct();
            list_todos.Items.Add(n);
            list_todos.SelectedIndex = list_todos.Items.Count - 1;
            backup.SaveNote(n);
            SetWindowHeight();
            EditNote();
        }

        private void RemoveNote()
        {
            int idx = list_todos.SelectedIndex;
            if (idx != -1)
            {
                NoteStruct note = list_todos.Items[idx] as NoteStruct;
                list_todos.Items.RemoveAt(idx);
                note.DeleteNote();
                backup.DeleteNote(note);
            }
            SetWindowHeight();
        }

        private void MakeNoteDone()
        {
            int idx = list_todos.SelectedIndex;
            if (idx != -1)
            {
                NoteStruct note = list_todos.Items[idx] as NoteStruct;
                note.MakeDone();
                note.CheckPriority();
                backup.SaveNote(note);
                if (sorted) Sort(lastOrder);
                else RefreshItemsView();
            }
        }

        private void RestoreDeletedNote()
        {
            if (backup.LastDeleted == null) return;
            backup.LastDeleted.CheckPriority();
            backup.SaveNote(backup.LastDeleted);
            list_todos.Items.Add(backup.LastDeleted);
            backup.LastDeleted = null;
            SetWindowHeight();
            RefreshItemsView();
        }

        private void EditNote()
        {
            if (list_todos.SelectedIndex == -1) return;
            NoteStruct note = list_todos.SelectedItem as NoteStruct;
            NoteEditor nedit = new NoteEditor(note);
            nedit.ShowDialog();
            backup.SaveNote(note);
            note.CheckPriority();
            if (sorted) Sort(lastOrder);
            else RefreshItemsView();
        }

        public void RefreshItemsView()
        {
            list_todos.Items.Refresh();
        }

        private void Exit()
        {
            MessageBoxResult res = MessageBox.Show(Res("s_dyrwtexit"), Res("s_closedln"), MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes) this.Close();
        }

        private void Sort(ListSortDirection Direction)
        {
            list_todos.Items.SortDescriptions.Clear();
            list_todos.Items.SortDescriptions.Add(new SortDescription("RemainingMinutes", Direction));
            RefreshItemsView();
            sorted = true;
        }

        private void border_header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void img_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Exit();
        }

        private void img_del_todo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RemoveNote();
        }

        private void img_done_todo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MakeNoteDone();
        }
        
        private void img_settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            settings.Show();
        }

        private void img_add_note_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddNewNote();
        }

        private void img_edit_todo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditNote();
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete: RemoveNote(); break;
                case Key.Enter: AddNewNote(); break;
                case Key.Escape: Exit(); break;
            }
        }

        private void img_undo_del_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RestoreDeletedNote();
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RestoreDeletedNote();
        }

        private void SwitchOrder()
        {
            if (lastOrder == ListSortDirection.Ascending)
            {
                img_orderby.Source = img_order_desc;
                lastOrder = ListSortDirection.Descending;
            }
            else
            {
                img_orderby.Source = img_order_asc;
                lastOrder = ListSortDirection.Ascending;
            }
            Sort(lastOrder);
        }

        private void img_orderby_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchOrder();
        }

        private void img_info_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            info.Show();
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            info.Close();
            settings.Close();
        }

    }
}
