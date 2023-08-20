using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Moos.Framework.Controls
{
    public class ColumnDefinition
    {
        public Position Position { set; get; }
        public int ActualHeight { get; }
        public int MaxHeight { get; set; }
        public int MinHeight { get; set; }
        public int Offset { get; }
        public GridLength Width { set; get; }

        public ColumnDefinition()
        {
            Position = new Position();
            Width = new GridLength(1, GridUnitType.Star);
        }

        public override string ToString()
        {
            return $"X: {Position.X}, Y: {Position.Y}, Width: {Position.Width}, Height: {Position.Height}";
        }
    }
}
