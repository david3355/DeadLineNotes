using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Media;

namespace DeadLineNotes
{
    class NoteBackup
    {
        private static NoteBackup instance;

        public static NoteBackup GetInstance()
        {
            if (instance == null) instance = new NoteBackup();
            return instance;
        }

        private NoteBackup()
        {
            doc = new XmlDocument();
            filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (!InitBackup())
                doc.Load(filePath);
        }

        private NoteStruct lastDeleted;

        private XmlDocument doc;
        private const string fileName = "savednotes.dln";
        private readonly string filePath;

        #region Constants

        private const string tag_root = "Notes";
        private const string atb_lastid = "last_id";
        private const string atb_a = "A";
        private const string atb_r = "R";
        private const string atb_g = "G";
        private const string atb_b = "B";
        private const string atb_notifs_enabled = "notifications_enabled";

        private const string tag_note = "Note";
        private const string atb_noteid = "id";
        private const string atb_note = "note";
        private const string atb_deadline = "deadline";
        private const string atb_lastmod = "lastmod";
        private const string atb_hasdl = "has_dl";
        private const string atb_notify = "notify";
        private const string atb_pinned = "pinned";
        private const string atb_done = "done";

        #endregion

        public NoteStruct LastDeleted
        {
            get { return lastDeleted; }
            set { lastDeleted = value; }
        }

        private bool InitBackup()
        {
            if (!File.Exists(filePath))
            {
                XmlNode root = doc.CreateElement(tag_root);
                (root as XmlElement).SetAttribute(atb_lastid, "0");
                doc.AppendChild(root);
                doc.Save(filePath);
                return true;
            }
            return false;
        }

        public int SaveNote(NoteStruct Note)
        {
            XmlElement root = doc.DocumentElement;
            if (Note.NoteID == -1)
            {
                XmlElement noteElement = doc.CreateElement(tag_note);
                int id = int.Parse(root.GetAttribute(atb_lastid));
                Note.SetID(++id);
                NoteToXml(Note, noteElement);
                root.AppendChild(noteElement);
                root.SetAttribute(atb_lastid, id.ToString());
            }
            else
            {
                bool found = false;
                foreach (XmlElement childElement in root.ChildNodes)
                {
                    if (childElement.GetAttribute(atb_noteid) == Note.NoteID.ToString())
                    {
                        NoteToXml(Note, childElement);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    XmlElement noteElement = doc.CreateElement(tag_note);
                    NoteToXml(Note, noteElement);
                    root.AppendChild(noteElement);
                }
            }
            doc.Save(fileName);
            return Note.NoteID;
        }

        /// <summary>
        /// Serialize (backup) Note object to XML
        /// </summary>
        private void NoteToXml(NoteStruct Note, XmlElement NoteElement)
        {
            NoteElement.SetAttribute(atb_noteid, Note.NoteID.ToString());
            NoteElement.SetAttribute(atb_note, Note.Note);
            NoteElement.SetAttribute(atb_deadline, String.Format("{0}-{1}-{2}-{3}-{4}", Note.DeadLine.Year, Note.DeadLine.Month, Note.DeadLine.Day, Note.DeadLine.Hour, Note.DeadLine.Minute));
            NoteElement.SetAttribute(atb_lastmod, String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Note.LastModified.Year, Note.LastModified.Month, Note.LastModified.Day, Note.LastModified.Hour, Note.LastModified.Minute, Note.LastModified.Second));
            NoteElement.SetAttribute(atb_hasdl, Note.HasDeadline ? "1" : "0");
            NoteElement.SetAttribute(atb_notify, Note.DoNotify.ToString());
            NoteElement.SetAttribute(atb_pinned, Note.Pinned.ToString());
            NoteElement.SetAttribute(atb_done, Note.Done.ToString());
        }

        public List<NoteStruct> ReadSavedNotes()
        {
            List<NoteStruct> notes = new List<NoteStruct>();
            XmlNode root = doc.DocumentElement;
            foreach (XmlElement childnode in root.ChildNodes)
            {
                notes.Add(XmlToNote(childnode));
            }
            return notes;
        }

        /// <summary>
        /// Parse note backup (XML) back to object
        /// </summary>
        private NoteStruct XmlToNote(XmlElement NoteElement)
        {
            string[] deadline = NoteElement.GetAttribute(atb_deadline).Split('-');
            string[] lastmod = NoteElement.GetAttribute(atb_lastmod).Split('-');
            string has_deadline = NoteElement.GetAttribute(atb_hasdl);
            string notify = NoteElement.GetAttribute(atb_notify);
            string pinned = NoteElement.GetAttribute(atb_pinned);
            string done = NoteElement.GetAttribute(atb_done);
            bool isPinned = pinned != String.Empty ? bool.Parse(pinned) : false;
            bool isDone = done != String.Empty ? bool.Parse(done) : false;
            NoteStruct note = new NoteStruct(NoteElement.GetAttribute(atb_note), new DateTime(int.Parse(deadline[0]), int.Parse(deadline[1]), int.Parse(deadline[2]), int.Parse(deadline[3]), int.Parse(deadline[4]), 0), int.Parse(NoteElement.GetAttribute(atb_noteid)), has_deadline == "1" ? true : false, isPinned, isDone);
            if (notify != String.Empty) note.SetDoNotify(bool.Parse(notify));
            note.SetLastModifiedDate(new DateTime(int.Parse(lastmod[0]), int.Parse(lastmod[1]), int.Parse(lastmod[2]), int.Parse(lastmod[3]), int.Parse(lastmod[4]), int.Parse(lastmod[5])));
            return note;
        }

        public void DeleteNote(NoteStruct Note)
        {
            lastDeleted = Note;
            XmlElement root = doc.DocumentElement;
            foreach (XmlElement noteElement in root.ChildNodes)
            {
                if (noteElement.GetAttribute(atb_noteid) == Note.NoteID.ToString())
                {
                    root.RemoveChild(noteElement);
                    break;
                }
            }
            doc.Save(filePath);
            // if archiválás ... save to another file
        }

        public void SaveNoteColors(int A, int R, int G, int B)
        {
            XmlElement root = doc.DocumentElement;
            root.SetAttribute(atb_a, A.ToString());
            root.SetAttribute(atb_r, R.ToString());
            root.SetAttribute(atb_g, G.ToString());
            root.SetAttribute(atb_b, B.ToString());
            doc.Save(filePath);
        }

        public Color GetSavedColor()
        {
            XmlElement root = doc.DocumentElement;
            Color originalColor = (Color)App.Current.TryFindResource("c_body");

            byte a, r, g, b;
            if (byte.TryParse(root.GetAttribute(atb_a), out a)
                && byte.TryParse(root.GetAttribute(atb_r), out r)
                && byte.TryParse(root.GetAttribute(atb_g), out g)
                && byte.TryParse(root.GetAttribute(atb_b), out b))
                return Color.FromArgb(a, r, g, b);
            else return Color.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }

        public void SaveNotificationsEnabled(bool NotificationsEnabled)
        {
            XmlElement root = doc.DocumentElement;
            root.SetAttribute(atb_notifs_enabled, NotificationsEnabled.ToString());
            doc.Save(filePath);
        }

        public bool GetNotificationsEnabled()
        {
            XmlElement root = doc.DocumentElement;
            bool enabled;
            return bool.TryParse(root.GetAttribute(atb_notifs_enabled), out enabled) ? enabled : true;
        }
    }

    class Archiver
    {

    }
}
