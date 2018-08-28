using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using PaperSoccer.Enums;

namespace PaperSoccer
{
    public class PaperGraphics
    {
        private int _width;
        private int _height;
        private Game _game;

        private readonly Color FirstPlayerColor = Color.DeepSkyBlue;
        private readonly Color SecondPlayerColor = Color.DarkOrange;
        private readonly Brush FirstPlayerBrush = Brushes.DeepSkyBlue;
        private readonly Brush SecondPlayerBrush = Brushes.DarkOrange;
        private readonly Pen FieldGridLinePen = new Pen(Color.LightBlue, 1.0f);
        private readonly Pen FieldOutLinePen = new Pen(Color.DimGray, 3.0f);
        private const float DotSize = 5f;

        public List<Rectangle> Hotspots { get; private set; }

        public PaperGraphics(Game game, int width, int height)
        {
            _game = game;
            _width = width;
            _height = height;
            SetHotspotsList();
        }

        private int PaperSquareSize
        {
            get
            {
                if (_game.Field.Width + 2 == 0) return 10;
                return (int)(1.00 * _width / (_game.Field.Width + 2));
            }
        }

        public Brush GetPlayerBrush(PlayerOrder playerDescription)
        {
            return playerDescription == PlayerOrder.First
                ? FirstPlayerBrush
                : SecondPlayerBrush;
        }

        public void DrawField(Graphics graphics)
        {
            DrawGrid(graphics);
            DrawFieldOutline(graphics);
            DrawMovesHistory(graphics);
            DrawConnections(graphics);
            DrawVerticesNumbers(graphics);
        }

        private void DrawPoint(Point where, float dot, Brush brush, Graphics graphics)
        {
            graphics.FillEllipse(brush,
                PaperSquareSize * (where.X + 1) - dot / 2,
                PaperSquareSize * (where.Y + 2) - dot / 2,
                dot,
                dot);
        }

        public void SetHotspotsList()
        {
            var hotspots = new List<Rectangle>();

            int hotSpotSize = PaperSquareSize / 2;
            int hotSpotSize2 = hotSpotSize / 2;

            var position = _game.CurrentPosition;

            foreach (var possiblePosition in _game.Field.PossibleMoves(position))
            {
                var pixelPossiblePosition = DrawingPosition(possiblePosition);

                hotspots.Add(new Rectangle(
                    pixelPossiblePosition.X - hotSpotSize2,
                    pixelPossiblePosition.Y - hotSpotSize2,
                    hotSpotSize,
                    hotSpotSize));
            }

            Hotspots = hotspots;
        }

        public void DrawGrid(Graphics graphics)
        {
            Point start;
            Point end;
            var paperSquareSize = PaperSquareSize;

            for (int x = paperSquareSize; x < _width; x += paperSquareSize)
            {
                start = new Point(x, 0);
                end = new Point(x, _height);
                graphics.DrawLine(FieldGridLinePen, start, end);
            }

            for (int y = paperSquareSize; y < _height; y += paperSquareSize)
            {
                start = new Point(0, y);
                end = new Point(_width, y);
                graphics.DrawLine(FieldGridLinePen, start, end);
            }
        }

        public void DrawFieldOutline(Graphics graphics)
        {
            var points = new List<Point>
            {
                DrawingPosition(0, 0),
                DrawingPosition(_game.Field.Width/2 - 1, 0),
                DrawingPosition(_game.Field.Width/2 - 1, -1),
                DrawingPosition(_game.Field.Width/2 + 1, -1),
                DrawingPosition(_game.Field.Width/2 + 1, 0),
                DrawingPosition(_game.Field.Width, 0),
                DrawingPosition(_game.Field.Width, _game.Field.Height),
                DrawingPosition(_game.Field.Width/2 + 1, _game.Field.Height),
                DrawingPosition(_game.Field.Width/2 + 1, _game.Field.Height + 1),
                DrawingPosition(_game.Field.Width/2 - 1, _game.Field.Height + 1),
                DrawingPosition(_game.Field.Width/2 - 1, _game.Field.Height),
                DrawingPosition(0, _game.Field.Height),
                DrawingPosition(0, 0)
            };

            graphics.DrawLines(FieldOutLinePen, points.ToArray());

            const float dot = DotSize;

            graphics.FillEllipse(Brushes.Black, PaperSquareSize * (_game.Field.Width / 2 + 1) - dot / 2,
                PaperSquareSize * (_game.Field.Height / 2 + 2) - dot / 2, dot, dot);
        }

        public Point DrawingPosition(int x, int y)
        {
            return new Point(PaperSquareSize * (x + 1), PaperSquareSize * (y + 2));
        }

        public Point DrawingPosition(Point p)
        {
            return new Point(PaperSquareSize * (p.X + 1), PaperSquareSize * (p.Y + 2));
        }

        public Point FieldPosition(Point p)
        {
            return new Point(p.X / PaperSquareSize - 1, p.Y / PaperSquareSize - 2);
        }

        public void DrawMovesHistory(Graphics graphics)
        {
            foreach (var move in _game.MovesHistory)
            {
                DrawMove(move, graphics);
            }
        }

        public void DrawMove(Move move, Graphics graphics)
        {
            var playerPen = GetPlayerPen(move);
            graphics.DrawLine(playerPen, DrawingPosition(move.From), DrawingPosition(move.To));

            var brush = GetPlayerBrush(_game.Player.Order);
            DrawPoint(move.To, DotSize, brush, graphics);

            if (_game.IsGameOver)
            {
                GameOver(_game.WonBy, graphics);
                Hotspots.Clear();
            }
            else
            {
                SetHotspotsList();
            }
        }

        private Pen GetPlayerPen(Move move)
        {
            return move.PlayerOrder == PlayerOrder.First
                ? new Pen(FirstPlayerColor, 2.0f)
                : new Pen(SecondPlayerColor, 2.0f);
        }

        public void GameOver(WonBy condition, Graphics graphics)
        {
            var font = new Font(FontFamily.GenericSansSerif, 24f);

            switch (condition)
            {
                case WonBy.Goal:
                    {
                        Point text = DrawingPosition(0, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.Player.Order + " wins", font,
                            Brushes.Black, (text));
                        Point nextLine = new Point(text.X, text.Y + 100);
                        graphics.DrawString("    by Goal!", font, Brushes.Brown, (nextLine));
                    }
                    break;
                case WonBy.Suicide:
                    {
                        _game.Player.Flip();
                        Point text = DrawingPosition(0, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.Player.Order + " wins", font,
                            Brushes.Black, (text));
                        Point nextLine = new Point(text.X, text.Y + 100);
                        graphics.DrawString("   by Suicide goal", font, Brushes.Black, (nextLine));
                    }
                    break;
                case WonBy.Block:
                    {
                        Point text = DrawingPosition(2, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.Player.Order + " wins", font,
                            Brushes.Black, (text));
                        Point nextLine = new Point(text.X, text.Y + 100);
                        graphics.DrawString("   by Block!", font, Brushes.Black, (nextLine));
                    }
                    break;
            }
        }

        [Conditional("DEBUG")]
        private void DrawHotSpots(Graphics graphics, Point position)
        {
            foreach (var item in Hotspots)
            {
                var where = new Point(item.Left + item.Width / 2, item.Top + item.Height / 2);
                DrawPoint(where, 16, Brushes.AliceBlue, graphics);
            }
        }

        [Conditional("DEBUG")]
        private void DrawConnections(Graphics graphics)
        {
            var movePen = new Pen(Color.Gray, 1.0f);

            for (int v = 0; v < _game.Field.GetVertices(); v++)
            {
                foreach (var w in _game.Field.GetAdjacencyList(v))
                {
                    graphics.DrawLine(movePen, DrawingPosition(_game.Field.Position(v)),
                        DrawingPosition(_game.Field.Position(w)));
                }
            }
        }

        [Conditional("DEBUG")]
        private void DrawVerticesNumbers(Graphics graphics)
        {
            var font = new Font(FontFamily.GenericSansSerif, 8f);

            for (int x = 0; x <= _game.Field.Width; x++)
            {
                for (int y = 0; y <= _game.Field.Height + 1; y++)
                {
                    var textPoint = DrawingPosition(x, y);
                    textPoint.Y -= 16;
                    graphics.DrawString(_game.Field.Vertex(x, y).ToString(), font, Brushes.Black, (textPoint));
                }
            }
        }
    }
}
