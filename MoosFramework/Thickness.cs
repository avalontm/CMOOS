namespace Moos.Framework
{
    public class Thickness
    {
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }

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