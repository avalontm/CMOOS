namespace Moos.Framework
{
    public class Thickness
    {
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }

        public Thickness(int margin)
        {
            Top = margin;
            Right = margin;
            Bottom = margin;
            Left += margin;
        }

        public Thickness(int right, int top, int left, int bottom)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left += left;
        }
    }
}