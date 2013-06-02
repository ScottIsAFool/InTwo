using System;
using System.Globalization;
using System.Windows.Data;
using Nokia.Music.Types;

namespace InTwo.Converters
{
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var retVal = string.Empty;
            if (value == null) return retVal;

            if (value is Artist[])
            {
                var artists = (Artist[]) value;
                var i = 0;
                foreach (var artist in artists)
                {
                    retVal += artist.Name;
                    i++;
                    if (i != artists.Length)
                    {
                        retVal += ", ";
                    }
                }

                return retVal;
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
