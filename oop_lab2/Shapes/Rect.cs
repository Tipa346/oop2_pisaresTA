using System.Drawing;

namespace Lab2.Shapes
{
    public class Rect : Shape
    {
        public Rect(Point start, Color color, int width)
            : base(start, color, width)
        {
        }
        public override void Draw(Graphics g)
        {
            using Pen pen = new Pen(Color, Width);

            g.DrawRectangle(pen, Bounds());
        }
    }
}