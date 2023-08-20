namespace System.Windows.Media
{
    public class SolidColorBrush : Brush
    {
        internal enum SerializationBrushType : byte
        {
            Unknown,
            KnownSolidColor,
            OtherColor
        }
    }
}
