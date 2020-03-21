using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace DeadLineNotes
{
    public class NoteStruct
    {
        public NoteStruct(string NoteText, DateTime Deadline, int ID, bool Has_Deadline, bool Pinned, bool Done)
        {
            id = ID;
            note = NoteText;
            deadline = Deadline;
            has_deadline = Has_Deadline;
            warn_turned_off = DateTime.Now;
            doNotify = true;
            pinned = Pinned;
            done = Done;
            CheckPriority();
        }

        public NoteStruct()
            : this(String.Empty, DateTime.Now, -1, true, false, false)
        {
            default_note = MainWindow.Res("s_defaultnote");
            note = default_note;
        }

        private int id;
        private string note;
        private DateTime deadline;
        private DateTime last_modified;
        private DateTime warn_turned_off;
        private bool has_deadline;
        private Priority priority;
        private bool hasNoteWarm;
        private string default_note;
        private bool doNotify;
        private bool pinned;
        private bool done;

        private const int WARN_PRIORITY = 5;

        public int NoteID
        {
            get { return id; }
        }

        public string Note
        {
            get { return note; }
        }

        public DateTime DeadLine
        {
            get { return deadline; }
        }

        public DateTime LastModified
        {
            get { return last_modified; }
        }

        public bool HasDeadline
        {
            get { return has_deadline; }
        }

        public TimeSpan Countdown
        {
            get { return deadline - DateTime.Now; }
        }

        public bool DoNotify
        {
            get { return doNotify; }
        }

        public bool Pinned
        {
            get { return pinned; }
        }

        public bool Done
        {
            get { return done; }
        }

        /// <summary>
        /// Order notes by this property
        /// </summary>
        public double RemainingMinutes
        {
            get
            {
                if (pinned) return 0;
                if (!has_deadline || Countdown.TotalSeconds <= 0)
                {
                    if (!done) return 0;
                    return int.MaxValue;
                }
                return Countdown.TotalSeconds;
            }
        }

        public string ShortNote
        {
            get
            {
                if (note == String.Empty) return "-";
                if (note.Length > 15)
                    return note.Substring(0, 15) + "...";
                return note;
            }
        }

        public string ShortDeadline
        {
            get
            {
                if (!has_deadline) return "-";
                return deadline.ToString(MainWindow.Res("s_dateform"));
            }
        }

        public string FullDeadline
        {
            get
            {
                if (!has_deadline) return "-";
                TimeSpan cd = Countdown;
                if (cd.TotalSeconds <= 0) cd = new TimeSpan(0, 0, 0, 0);
                return ShortDeadline + " " + deadline.ToLongTimeString() + " " + String.Format(MainWindow.Res("s_remtime"), cd.Days, cd.Hours, cd.Minutes);
            }
        }

        public bool TimeToWarn
        {
            get
            {
                TimeSpan past = DateTime.Now - warn_turned_off;
                if (!hasNoteWarm && Countdown.TotalSeconds > 0 && priority.Value >= WARN_PRIORITY && past.TotalMinutes > priority.WarnCycle) return true;
                return false;
            }
        }

        public Visibility DoneCheckVisibility
        {
            get { return Countdown.TotalSeconds <= 0 && !done ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Color PrioritySign
        {
            get
            {
                return priority.Color;
            }
        }

        public void SetID(int ID)
        {
            id = ID;
        }

        public void ChangeNote(string NoteText)
        {
            note = NoteText;
            last_modified = DateTime.Now;
        }

        public void ChangeDeadLine(DateTime DeadlineDate)
        {
            has_deadline = true;
            deadline = DeadlineDate;
            warn_turned_off = DateTime.Now;
            last_modified = DateTime.Now;
        }

        public void SetNoDeadLine()
        {
            has_deadline = false;
            last_modified = DateTime.Now;
        }

        public void SetWarn()
        {
            hasNoteWarm = true;
        }

        public void WarnClosed()
        {
            hasNoteWarm = false;
            warn_turned_off = DateTime.Now;
        }

        public void SetLastModifiedDate(DateTime Date)
        {
            last_modified = Date;
        }

        public void DeleteNote()
        {
            last_modified = DateTime.Now;
        }

        public Priority CheckPriority()
        {
            double hours;
            if (!has_deadline) hours = -1;
            else
            {
                TimeSpan t = deadline - DateTime.Now;
                hours = t.TotalHours;
            }
            if (priority == null) priority = new Priority(hours, pinned, done);
            else priority.Adjust(hours, pinned, done);
            return priority;
        }

        public void SetDoNotify(bool notify)
        {
            doNotify = notify;
        }

        public void SetPinned(bool Pinned)
        {
            pinned = Pinned;
        }

        public void SetDone(bool Done)
        {
            done = Done;
        }

        public void MakeDone()
        {
            done = true;
        }
    }

    public class Priority
    {
        public Priority(double Hours, bool Pinned, bool Done)
        {
            Adjust(Hours, Pinned, Done);
        }

        private int value;
        private Color color;
        private double hours;

        public int Value { get { return value; } }
        public Color Color { get { return color; } }

        public double WarnCycle
        {
            get
            {
                double wc = hours * 60 / 3;
                return wc;
            }
        }

        public void Adjust(double Hours, bool Pinned, bool Done)
        {
            hours = Hours;
            if (Hours <= 0)
            {
                if (Pinned || !Done) value = 10;
                else value = 0; // Nincs határidő
            }
            else if (Hours < 1) value = 10;
            else if (Hours < 2) value = 9;
            else if (Hours < 5) value = 8;
            else if (Hours < 12) value = 7;
            else if (Hours < 24) value = 6; // One day
            else if (Hours < 48) value = 5; // Two days
            else if (Hours < 168) value = 4; // One week
            else if (Hours < 720) value = 3; // One month
            else if (Hours < 1440) value = 2; // Two month
            else value = 1;
            AdjustColor();
        }

        private void AdjustColor()
        {
            switch (value)
            {
                case 0: color = Color.FromArgb(150, 255, 255, 255); break;
                case 1: color = Color.FromRgb(220, 220, 220); break;
                case 2: color = Color.FromRgb(0, 220, 0); break;
                case 3: color = Color.FromRgb(0, 175, 0); break;
                case 4: color = Color.FromRgb(0, 125, 0); break;
                case 5: color = Color.FromRgb(255, 200, 0); break;
                case 6: color = Color.FromRgb(255, 135, 0); break;
                case 7: color = Color.FromRgb(255, 77, 0); break;
                case 8: color = Color.FromRgb(230, 80, 75); break;
                case 9: color = Color.FromRgb(255, 40, 40); break;
                case 10: color = Color.FromRgb(255, 0, 0); break;
            }
        }
    }    
}
