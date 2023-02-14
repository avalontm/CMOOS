namespace System.Moos
{
    public class Thickness
    {
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }

        public Thickness(int margin)
        {
            this.Top = margin;
            this.Right = margin;
            this.Bottom = margin;
            this.Left += margin;
        }

        public Thickness(int right, int top, int left, int bottom)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left += left;
        }
    }
}