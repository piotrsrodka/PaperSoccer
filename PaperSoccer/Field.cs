using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PaperSoccer
{
    /* Field is a kind of Graph.
     * Connection are only initialized beetween geometric adjacent points
     */

    public class Field : Graph
    {
        private int _graphWidth => Width + 3;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Point MiddlePoint => new Point(Width / 2, Height / 2);

        public Field(int width, int height) : base((width + 3) * (height + 3))
        {
            if (width % 2 != 0) width++;
            if (height % 2 != 0) height++;
            if (width < 4 || width > 24) width = 8;
            if (height < 4 || height > 30) height = 10;

            Width = width;
            Height = height;

            InnerMesh(width, height);

            /* Top Goal */
            foreach (var p in Neighbours(new Point(width / 2, 0)))
            {
                AddEdge(Vertex(width / 2, 0), Vertex(p));
            }

            AddEdge(Vertex(width / 2 - 1, 0), Vertex(width / 2, -1));
            AddEdge(Vertex(width / 2 + 1, 0), Vertex(width / 2, -1));

            /* Bottom Goal */
            foreach (var p in Neighbours(new Point(width / 2, height)))
            {
                AddEdge(Vertex(width / 2, height), Vertex(p));
            }

            AddEdge(Vertex(width / 2 - 1, height), Vertex(width / 2, height + 1));
            AddEdge(Vertex(width / 2 + 1, height), Vertex(width / 2, height + 1));

            /* Corners */
            AddEdge(Vertex(0, 1), Vertex(1, 0));
            AddEdge(Vertex(width - 1, 0), Vertex(width, 1));
            AddEdge(Vertex(0, height - 1), Vertex(1, height));
            AddEdge(Vertex(width - 1, height), Vertex(width, height - 1));
        }

        /* Mapping vertices into point and vice versa */

        public int Vertex(int x, int y)
        {
            return (x + 1) + (y + 1) * _graphWidth;
        }

        public int Vertex(Point a)
        {
            return Vertex(a.X, a.Y);
        }

        public Point Position(int v)
        {
            return new Point((v % _graphWidth) - 1, v / _graphWidth - 1);
        }

        /* End mapping */

        public List<Point> PossibleMoves(Point position)
        {
            return GetAdjacencyList(Vertex(position)).Select(Position).ToList();
        }

        public int DegreeOf(Point point)
        {
            return base.DegreeOf(Vertex(point));
        }

        public bool IsMoveIntoTheVoid(Point to)
        {
            return DegreeOf(to) == 7; // first visit - can't bump
        }

        public bool IsStalemate(Point to, Point @from)
        {
            return !ExistEdge(Vertex(@from), Vertex(to));
        }

        private void InnerMesh(int width, int height)
        {
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    foreach (var w in Neighbours(new Point(x, y)))
                    {
                        AddEdge(Vertex(x, y), Vertex(w));
                    }
                }
            }
        }

        private IEnumerable<Point> Neighbours(Point position)
        {
            var positionList = new List<Point>
            {
                new Point(position.X, position.Y - 1),
                new Point(position.X + 1, position.Y - 1),
                new Point(position.X + 1, position.Y),
                new Point(position.X + 1, position.Y + 1),
                new Point(position.X, position.Y + 1),
                new Point(position.X - 1, position.Y + 1),
                new Point(position.X - 1, position.Y),
                new Point(position.X - 1, position.Y - 1)
            };
            return positionList;
        }
    }
}
