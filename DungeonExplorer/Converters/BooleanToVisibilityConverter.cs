using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DungeonExplorer.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // Converts bool -> Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag)
            {
                // If parameter is "Invert", flip the meaning
                bool invert = parameter as string == "Invert";

                if (invert)
                    flag = !flag;

                return flag ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        // Not used (one-way binding)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
