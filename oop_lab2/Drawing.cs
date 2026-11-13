using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using Lab2.Shapes;

namespace Lab2
{
    public class Drawing
    {
        private readonly List<Shape> _shapes = new();

        public IReadOnlyList<Shape> Shapes => _shapes;

        public void Add(Shape shape)
        {
            _shapes.Add(shape);
        }

        public void Undo()
        {
            if (_shapes.Count > 0)
                _shapes.RemoveAt(_shapes.Count - 1);
        }

        public void Clear()
        {
            _shapes.Clear();
        }

        public void Draw(Graphics g)
        {
            foreach (Shape shape in _shapes)
            {
                shape.Draw(g);
            }
        }

        public void Save(string path)
        {
            var data = _shapes.Select(shape => new ShapeData
            {
                Type = shape.GetType().Name,
                Start = shape.Start,
                End = shape.End,
                Color = shape.Color.ToArgb(),
                Width = shape.Width,

                Points = shape is Pencil pencil
                    ? pencil.Points.ToList()
                    : new List<Point>()

            }).ToList();

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            string json = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<List<ShapeData>>(json);

            if (data == null)
                throw new InvalidDataException("Некорректный JSON-файл.");

            var loadedShapes = new List<Shape>();

            foreach (ShapeData item in data)
            {
                Color color = Color.FromArgb(item.Color);

                Shape shape = ShapeFactory.Create(
                    item.Type,
                    item.Start,
                    color,
                    item.Width
                );

                if (shape is Pencil pencil)
                {
                    foreach (Point point in item.Points.Skip(1))
                    {
                        pencil.MoveEnd(point);
                    }
                }
                else
                {
                    shape.MoveEnd(item.End);
                }

                loadedShapes.Add(shape);
            }

            _shapes.Clear();
            _shapes.AddRange(loadedShapes);
        }
    }

    public class ShapeData
    {
        public string Type { get; set; } = "";

        public Point Start { get; set; }

        public Point End { get; set; }

        public int Color { get; set; }

        public int Width { get; set; }

        public List<Point> Points { get; set; } = new();
    }
}