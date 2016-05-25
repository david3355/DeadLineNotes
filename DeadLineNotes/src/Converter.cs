using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DeadLineNotes
{
    class Converter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Helper.ObjToInt(value) + Helper.ObjToInt(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 1;
        }
    }

    class Helper
    {
        public static int ObjToInt(object obj)
        {
            return Convert.ToInt32(obj);
        }
    }
}
