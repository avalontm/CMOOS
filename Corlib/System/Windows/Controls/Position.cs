namespace System.Windows.Controls
{
    public class Position
    {
        public int X { set; get; }
        public int Y { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }

        public Position()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public Position(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }
}
