using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DeepCore
{
    public class RankFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int f = (int)value;

            switch (f)
            {
                case 1:
                    return "Assets/stack-no-1.png";
                case 2:
                    return "Assets/stack-no-2.png";
                case 3:
                    return "Assets/stack-no-3.png";

                default: 
                    return "Assets/stack-line.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
