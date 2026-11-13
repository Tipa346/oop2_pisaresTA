using System.Collections.Generic;
using System.Drawing;

namespace Lab2.Shapes
{
    public class Pencil : Shape
    {
        private readonly List<Point> _points = new();

        public IReadOnlyList<Point> Points => _points;

        public Pencil(Point start, Color color, int width)
            : base(start, color, width)
        {
            _points.Add(start);
        }

        public override void MoveEnd(Point point)
        {
            base.MoveEnd(point);
            _points.Add(point);
        }

        public override void Draw(Graphics g)
        {
            if (_points.Count < 2)
                return;

            using Pen pen = new Pen(Color, Width);

            g.DrawLines(pen, _points.ToArray());
        }
    }
}