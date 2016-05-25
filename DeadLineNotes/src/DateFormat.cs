using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadLineNotes
{
    class DateFormat
    {
        public static int NumberOfDateForms { get { return dateformats.Length; } }

        public static string GetDateForm(int i)
        {
            if (i < NumberOfDateForms) return dateformats[i];
            return "yyyy.MM.dd.";
        }

        public static string[] dateformats = 
        {
           "yyyy.MM.dd.",
           "yyyy. MMM dd.",
           "yyyy.MM.dd",
           "yyyy. MMMM dd.",
           "MM.dd.",
           "MMMdd.",
           "MMMM.dd.",
           "dd.MM.yyyy",
           "dd.MM.yyyy.",
           "dd. MMM yyyy",
           "dd. MMMM yyyy"
        };
    }
}
