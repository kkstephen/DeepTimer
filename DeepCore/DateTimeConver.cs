using System;
using System.Collections.Generic;
using System.Globalization; 
using System.Windows.Data;

namespace DeepCore
{
    public class DateTimeConver : IValueConverter
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            DateTime date = new DateTime((long)value);

            return date.ToString("mm:ss.fff");
        }
 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
