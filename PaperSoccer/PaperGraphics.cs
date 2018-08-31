using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using PaperSoccer.Enums;

namespace PaperSoccer
{
    public class PaperGraphics
    {
        private int _width;
        private int _height;
        private Game _game;

        private const float DotSize = 5f;
        private readonly Color FirstPlayerColor = Color.DeepSkyBlue;
        private readonly Color SecondPlayerColor = Color.DarkOrange;
        private readonly Brush FirstPlayerBrush = Brushes.DeepSkyBlue;
        private readonly Brush SecondPlayerBrush = Brushes.DarkOrange;
        private readonly Pen FieldGridLinePen = new Pen(Color.LightBlue, 1.0f);
        private readonly Pen FieldOutLinePen = new Pen(Color.DimGray, 3.0f);

        /* Graphical API */

        public List<Rectangle> Hotspots { get; private set; }

        public PaperGraphics(Game game, int width, int height)
        {
            _game = game;
            _width = width;
            _height = height;
            SetHotspotsList();
        }

        public void DrawMovesHistory(Graphics graphics)
        {
            foreach (var move in _game.MovesHistory)
            {
                DrawMove(move, graphics);
            }
        }

        public void DrawLastMoves(Graphics graphics)
        {
            bool isComputerBulk = _game.LastMoves.Count > 1;

            foreach (var move in _game.LastMoves)
            {
                if (isComputerBulk) Thread.Sleep(100);

                DrawMove(move, graphics);
            }
        }

        public void DrawMove(Move move, Graphics graphics)
        {
            var playerPen = GetPlayerPen(move);
            graphics.DrawLine(playerPen, DrawingPosition(move.From), DrawingPosition(move.To));

            var brush = GetPlayerBrush(_game.PlayerTurn);
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

        public void DrawField(Graphics graphics)
        {
            DrawGrid(graphics);
            DrawFieldOutline(graphics);
            DrawMovesHistory(graphics);
            DrawConnections(graphics);
            DrawVerticesNumbers(graphics);
        }

        public Point GetBallLocation(int ballSize)
        {
            return new Point(GetPaperSquareSize() * (_game.CurrentPosition.X + 1) - ballSize / 2,
                GetPaperSquareSize() * (_game.CurrentPosition.Y + 2) - ballSize / 2);
        }

        public Point FieldPosition(Point p)
        {
            return new Point(p.X / GetPaperSquareSize() - 1, p.Y / GetPaperSquareSize() - 2);
        }

        /* API ends */

        private void SetHotspotsList()
        {
            var hotspots = new List<Rectangle>();

            int hotSpotSize = GetPaperSquareSize() / 2;
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

        private Brush GetPlayerBrush(PlayerOrder playerDescription)
        {
            return playerDescription == PlayerOrder.First
                ? FirstPlayerBrush
                : SecondPlayerBrush;
        }

        private void DrawPoint(Point where, float dot, Brush brush, Graphics graphics)
        {
            graphics.FillEllipse(brush,
                GetPaperSquareSize() * (where.X + 1) - dot / 2,
                GetPaperSquareSize() * (where.Y + 2) - dot / 2,
                dot,
                dot);
        }

        private int GetPaperSquareSize()
        {
            if (_game.Field.Width + 2 == 0) return 10;
            return (int)(1.00 * _width / (_game.Field.Width + 2));
        }

        private void DrawGrid(Graphics graphics)
        {
            Point start;
            Point end;

            for (int x = GetPaperSquareSize(); x < _width; x += GetPaperSquareSize())
            {
                start = new Point(x, 0);
                end = new Point(x, _height);
                graphics.DrawLine(FieldGridLinePen, start, end);
            }

            for (int y = GetPaperSquareSize(); y < _height; y += GetPaperSquareSize())
            {
                start = new Point(0, y);
                end = new Point(_width, y);
                graphics.DrawLine(FieldGridLinePen, start, end);
            }
        }

        private void DrawFieldOutline(Graphics graphics)
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

            graphics.FillEllipse(Brushes.Black, GetPaperSquareSize() * (_game.Field.Width / 2 + 1) - dot / 2,
                GetPaperSquareSize() * (_game.Field.Height / 2 + 2) - dot / 2, dot, dot);
        }

        private Point DrawingPosition(int x, int y)
        {
            return new Point(GetPaperSquareSize() * (x + 1), GetPaperSquareSize() * (y + 2));
        }

        private Point DrawingPosition(Point p)
        {
            return new Point(GetPaperSquareSize() * (p.X + 1), GetPaperSquareSize() * (p.Y + 2));
        }

        private Pen GetPlayerPen(Move move)
        {
            return move.PlayerOrder == PlayerOrder.First
                ? new Pen(FirstPlayerColor, 2.0f)
                : new Pen(SecondPlayerColor, 2.0f);
        }

        private void GameOver(WonBy condition, Graphics graphics)
        {
            var font = new Font(FontFamily.GenericSansSerif, 24f);

            switch (condition)
            {
                case WonBy.Goal:
                    {
                        Point text = DrawingPosition(0, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.PlayerTurn + " wins", font,
                            Brushes.Black, (text));
                        Point nextLine = new Point(text.X, text.Y + 100);
                        graphics.DrawString("    by Goal!", font, Brushes.Brown, (nextLine));
                    }
                    break;
                case WonBy.Suicide:
                    {
                        Point text = DrawingPosition(0, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.PlayerTurn + " loses", font,
                            Brushes.Black, (text));
                        Point nextLine = new Point(text.X, text.Y + 100);
                        graphics.DrawString("   by Suicide goal", font, Brushes.Black, (nextLine));
                    }
                    break;
                case WonBy.Block:
                    {
                        Point text = DrawingPosition(2, _game.Field.Height / 3);
                        graphics.DrawString(" Player " + _game.PlayerTurn + " wins", font,
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
