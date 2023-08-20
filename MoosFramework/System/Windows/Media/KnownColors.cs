namespace System.Windows.Media
{
    public class KnownColors
    {
        public static Brush BrushFromUint(uint argb)
        {
            return new Brush(argb);
        }
    }
}
