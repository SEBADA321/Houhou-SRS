﻿using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kanji.Interface.Converters
{
    public class StringAppendConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string output = string.Empty;
            if (values != null)
            {
                foreach (object value in values)
                {
					if (value != null)
						output += value.ToString();
                }
            }

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
