using System;
using System.Windows;
using System.Windows.Data;

namespace TestWPF.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            return Enum.GetName((value.GetType()), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(targetType, (string)value);
        }
        #endregion
    }
}
