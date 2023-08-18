using System;
using System.Globalization;
using System.Windows;

namespace System.Windows
{
    public class ThicknessConverter
    {
        public object ConvertFrom(object context, CultureInfo cultureInfo, object source)
        {
            Thickness thickness = new Thickness();

            if (string.IsNullOrEmpty(source.ToString()))
            {
                return thickness;
            }

            thickness = new Thickness(Convert.ToInt32(source.ToString()));

            return thickness;
        }
    }
}
