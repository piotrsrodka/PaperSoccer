using System.Collections.Generic;
using System.Drawing;
using PaperSoccer.Enums;

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
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        public PlayerOrder PlayerTurn { get; set; }
        public Point CurrentPosition { get; set; }
        public List<Move> LastMoves { get; set; }
        public WonBy WonBy { get; set; }
        public List<Move> MovesHistory { get; set; }

        public Game() : this(DefaultWidth, DefaultHeight, "Player 1", PlayerNature.Human,
            "Player 2", PlayerNature.Human)
        {
        }

        public Game(int width, int height, string player1Name, PlayerNature player1Nature,
            string player2Name, PlayerNature player2Nature)
        {
            Width = width;
            Height = height;
            PlayerOne = new Player(player1Name, player1Nature);
            PlayerTwo = new Player(player2Name, player2Nature);
            PlayerTurn = PlayerOrder.First;
            Field = new Field(width, height);
            CurrentPosition = Field.MiddlePoint;
            MovesHistory = new List<Move>();
            LastMoves = new List<Move>();
        }

        public void Flip()
        {
            if (PlayerTurn == PlayerOrder.First)
            {
                PlayerTurn = PlayerOrder.Second;
            }
            else if (PlayerTurn == PlayerOrder.Second)
            {
                PlayerTurn = PlayerOrder.First;
            }
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
            LastMoves.Clear();
            LastMoves.Add(new Move(PlayerTurn, to, from));
            MovesHistory.AddRange(LastMoves);
            VictoryConditions(to);

            if (IsGameOver)
            {
                NumberOfMoves++;
                return;
            }
            
            if (Field.IsMoveIntoTheVoid(to))
            {
                Flip();
                NumberOfMoves++;

                IsComputerTurn = PlayerTurn == PlayerOrder.Second &&
                                  PlayerTwo.Nature == PlayerNature.Computer;
            }
        }

        public void ComputerMove()
        {
            var goal = new Point(Width / 2, Height + 1);
            IComputer computer = new Walter();
            Stack<int> pathToGoal = computer.GetPathToGoal(Field, CurrentPosition, goal);

            if (pathToGoal == null)
            {
                Flip();
                WonBy = WonBy.Block;
                IsGameOver = true;
                IsComputerTurn = false;
                return;
            }

            int pop = pathToGoal.Pop();
            pop = pathToGoal.Pop();

            LastMoves.Clear();
            MoveComputer(CurrentPosition, Field.Position(pop), pathToGoal);
            MovesHistory.AddRange(LastMoves);

            Flip();
            NumberOfMoves++;
            IsComputerTurn = false;
        }

        /* Iteration is human thing. Recursion divine */
        private void MoveComputer(Point from, Point to, Stack<int> pathToGoal)
        {
            Field.RemoveEdge(Field.Vertex(from), Field.Vertex(to));
            CurrentPosition = to;
            LastMoves.Add(new Move(PlayerTurn, to, from));
            
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
                WonBy = PlayerTurn == PlayerOrder.First ? WonBy.Goal : WonBy.Suicide;
                IsGameOver = true;
            }
            else if (isBottomGoal)
            {
                WonBy = PlayerTurn == PlayerOrder.First ? WonBy.Suicide : WonBy.Goal;
                IsGameOver = true;
            }
            else if (Field.DegreeOf(Field.Vertex(to)) == 0)
            {
                WonBy = WonBy.Block;
                Flip();
                IsGameOver = true;
            }
        }
    }
}
