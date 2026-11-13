using System.Drawing;

namespace Lab2.Shapes
{
    public class Ellipse : Shape
    {
        public Ellipse(Point start, Color color, int width)
            : base(start, color, width)
        {
        }
        public override void Draw(Graphics g)
        {
            using Pen pen = new Pen(Color, Width);

            g.DrawEllipse(pen, Bounds());
        }
    }
}