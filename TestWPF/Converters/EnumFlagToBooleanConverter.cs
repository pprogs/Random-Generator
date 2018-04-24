using System;
using System.Windows;
using System.Windows.Data;

namespace TestWPF.Converters
{
    public class EnumFlagToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            Enum o = (Enum)value;
            Enum p = (Enum)Enum.Parse(value.GetType(), parameterString);
  
            return o.HasFlag(p); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {       
            return DependencyProperty.UnsetValue;
        }
        #endregion
    }
}
