using System.Collections.Generic;
using System.Drawing;
using PaperSoccer.Enums;
using System.Threading;

namespace PaperSoccer
{
    public class Game
    {
        private const int DefaultWidth = 8;
        private const int DefaultHeight = 10;

        public bool IsGameOver { get; set; }
        public bool IsComputerTurn { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int NumberOfMoves { get; set; }

        public Field Field { get; set; }
        public Player Player { get; set; }
        public Point CurrentPosition { get; set; }
        public Move Move { get; set; }
        public WonBy WonBy { get; set; }
        public List<Move> MovesHistory { get; set; }

        public Game() : this(DefaultWidth, DefaultHeight, PlayerNature.Human)
        {
        }

        public Game(int width, int height, PlayerNature playerNature)
        {
            Width = width;
            Height = height;
            Player = new Player(playerNature);
            Field = new Field(width, height);
            CurrentPosition = Field.MiddlePoint;
            MovesHistory = new List<Move>();
            Move = new Move();
        }

        public void PlayerMove(Point to)
        {
            if (IsGameOver ||
                IsComputerTurn ||
                CurrentPosition.Equals(to) ||
                Field.IsStalemate(to, CurrentPosition))
            {
                return;
            }

            var from = CurrentPosition;
            Field.RemoveEdge(Field.Vertex(from), Field.Vertex(to));
            CurrentPosition = to;
            Move = new Move(Player.Order, to, from);
            MovesHistory.Add(Move);
            VictoryConditions(to);

            if (IsGameOver)
            {
                NumberOfMoves++;
                return;
            }
            
            if (Field.IsMoveIntoTheVoid(to))
            {
                Player.Flip();
                NumberOfMoves++;

                IsComputerTurn = Player.Order == PlayerOrder.Second &&
                                  Player.Nature == PlayerNature.Computer;

                if (IsComputerTurn)
                {
                    ComputerMove();
                }
            }
        }

        private void ComputerMove()
        {
            var bfs = new Bfs(Field, Field.Vertex(CurrentPosition));
            var goal = new Point(Width / 2, Height + 1);
            Stack<int> pathToGoal = bfs.PathTo(Field.Vertex(goal));

            if (pathToGoal == null)
            {
                Player.Flip();
                WonBy = WonBy.Block;
                IsGameOver = true;
                IsComputerTurn = false;
                return;
            }

            int pop = pathToGoal.Pop();
            pop = pathToGoal.Pop();

            MoveComputer(CurrentPosition, Field.Position(pop), pathToGoal);

            Player.Flip();
            NumberOfMoves++;
            IsComputerTurn = false;
        }

        /* Iteration is human thing. Recursion divine */
        private void MoveComputer(Point from, Point to, Stack<int> pathToGoal)
        {
            Field.RemoveEdge(Field.Vertex(from), Field.Vertex(to));
            CurrentPosition = to;
            Move = new Move(Player.Order, to, from);
            MovesHistory.Add(new Move(Player.Order, to, from));
            VictoryConditions(to);

            if (IsGameOver)
            {
                NumberOfMoves++;
                return;
            }

            if (Field.IsMoveIntoTheVoid(to))
            {
                return;
            }
            else // bump and move further
            {
                MoveComputer(CurrentPosition, Field.Position(pathToGoal.Pop()), pathToGoal);
            }
        }

        private void VictoryConditions(Point to)
        {
            bool isTopGoal = to.Y < 0;
            bool isBottomGoal = to.Y > Height;

            if (isTopGoal)
            {
                WonBy = Player.Order == PlayerOrder.First ? WonBy.Goal : WonBy.Suicide;
                IsGameOver = true;
            }
            else if (isBottomGoal)
            {
                WonBy = Player.Order == PlayerOrder.First ? WonBy.Suicide : WonBy.Goal;
                IsGameOver = true;
            }
            else if (Field.DegreeOf(Field.Vertex(to)) == 0)
            {
                WonBy = WonBy.Block;
                Player.Flip();
                IsGameOver = true;
            }
        }
    }
}
