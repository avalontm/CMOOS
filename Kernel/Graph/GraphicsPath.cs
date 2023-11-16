using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MOOS.Graph
{
    public class GraphicsPath
    {
        private List<Point> points = new List<Point>();

        public void AddLine(int x1, int y1, int x2, int y2)
        {
            if (x1 < x2)
            {
                for (int x = x1; x <= x2; x++)
                {
                    points.Add(new Point(x, y1));
                }
            }
            else
            {
                for (int x = x1; x >= x2; x--)
                {
                    points.Add(new Point(x, y1));
                }
            }
        }

        public void AddArc(int x, int y, int radius, int radiusCorner, int startAngle, int endAngle)
        {
            for (int i = startAngle; i <= endAngle; i++)
            {
                double angle = (double)i / 360.0f * Math.PI * 2.0;
                int x1 = (int)(x + radius * Math.Cos(angle));
                int y1 = (int)(y + radius * Math.Sin(angle));

                // If the angle is a multiple of 90, use the corner radius
                if (i % 90 == 0)
                {
                    radius = radiusCorner;
                }

                points.Add(new Point(x1, y1));
            }
        }

        public void CloseAll()
        {
            if (points.Count > 0)
            {
                points.Add(points[0]);
            }
        }

        public int PointCount
        {
            get { return points.Count; }
        }

        public Point this[int index]
        {
            get { return points[index]; }
            set { points[index] = value; }
        }
    }

}
