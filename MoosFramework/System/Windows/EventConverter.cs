using System.Globalization;

namespace System.Windows
{
    public class EventConverter
    {
        public object ConvertFrom(object context, CultureInfo cultureInfo, object source)
        {
            EventHandler SomeEvent = new EventHandler((EventHandler)source);
            return SomeEvent;
        }
    }
}
