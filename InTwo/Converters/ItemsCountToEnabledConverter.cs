using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace InTwo.Converters
{
    public class ItemsCountToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var count = 0;
            if (value is IList)
            {
                var list = (IList) value;
                count = list.Count;
            }
            else if (value.GetType().IsArray)
            {
                var list = (Array) value;
                count = list.Length;
            }
            else
            {
                return false;
            }

            var action = parameter == null ? "zero" : parameter.ToString();

            if (action.Equals("single"))
                return count == 1;
            if (action.Equals("multiple"))
                return count > 0;

            return count == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
