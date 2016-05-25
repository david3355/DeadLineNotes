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
using System.Windows.Threading;

namespace DeadLineNotes
{
    /// <summary>
    /// Interaction logic for NoteWarn.xaml
    /// </summary>
    public partial class NoteWarn : Window
    {
        public NoteWarn(NoteStruct Note)
        {
            InitializeComponent();
            Init(Note);
        }

        private NoteStruct ownNote;
        private DispatcherTimer updater;

        private void Init(NoteStruct Note)
        {
            updater = new DispatcherTimer();
            updater.Interval = MainWindow.CheckInterval;
            updater.Tick += Refresh;
            ownNote = Note;
            SetUI();
            updater.Start();
        }

        private void SetUI()
        {
            label_note.Content = ownNote.ShortNote;
            label_note.ToolTip = ownNote.Note;
            label_deadline.Content = ownNote.ShortDeadline;
            label_deadline.ToolTip = ownNote.FullDeadline;
            border_background.Background = new SolidColorBrush(ownNote.PrioritySign);
        }

        private void Refresh(object sender, EventArgs e)
        {
            SetUI();
        }

        private void CloseNoteWarn()
        {
            ownNote.WarnClosed();
            this.Close();
        }

        private void img_close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CloseNoteWarn();
        }

        private void border_background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete: CloseNoteWarn(); break;
            }
        }
    }
}
