using System;
using System.Collections.Generic;
using System.Drawing;
using Lab2.Shapes;

namespace Lab2
{
    public static class ShapeFactory
    {
        private static readonly Dictionary<string,
            Func<Point, Color, int, Shape>> Kinds = new()
        {
            ["Line"] = (p, c, w) => new Line(p, c, w),
            ["Rect"] = (p, c, w) => new Rect(p, c, w),
            ["Ellipse"] = (p, c, w) => new Ellipse(p, c, w),
            ["Pencil"] = (p, c, w) => new Pencil(p, c, w)
        };

        public static Shape Create(
            string kind,
            Point start,
            Color color,
            int width)
        {
            if (!Kinds.TryGetValue(kind, out var creator))
            {
                throw new ArgumentException(
                    "Неизвестная фигура: " + kind);
            }

            return creator(start, color, width);
        }
    }
}