using System;
using System.Globalization;
using System.Windows.Data;

namespace Timeline
{
    public class PlayButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
                return "Pause";
            }

            return "Play";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
