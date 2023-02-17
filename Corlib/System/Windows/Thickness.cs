namespace System.Windows
{
    public struct Thickness
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }

        public Thickness()
        {
            Top = 0;
            Left = 0;
            Bottom = 0;
            Right = 0;
        }

        public Thickness(int margin)
        {
            Top = margin;
            Left = margin;
            Bottom = margin;
            Right = margin;
        }

        public Thickness(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }
    }
}
