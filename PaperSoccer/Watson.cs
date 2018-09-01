using System;
using System.Collections.Generic;
using System.Drawing;

namespace PaperSoccer
{
    /// <summary>
    /// Medium computer opponent. Same as Walter, but do some forecasting into next move.
    /// </summary>
    class Watson : IComputer
    {
        private Game _game;

        public Watson(Game game)
        {
            _game = game;
        }

        public Stack<int> GetPathToGoal(Field field, Point currentPosition, Point goal)
        {
            var bfs = new BreadthFirstSearch(field, field.Vertex(currentPosition));
            var pathToGoal = bfs.PathTo(field.Vertex(goal));
            var result = new Stack<int>();

            _game.LastMoves.Clear();
            int maxMoves = 0;
            Point bestMove;

            foreach (var move in _game.Field.PossibleMoves(currentPosition))
            {
                bfs = new BreadthFirstSearch(field, field.Vertex(move));
                pathToGoal = bfs.PathTo(field.Vertex(goal));
                var from = _game.Field.Position(pathToGoal.Pop());
                var to = _game.Field.Position(pathToGoal.Pop());
                var lastMoves = MoveComputer(from, to, pathToGoal, goal).LastMoves;
                var numberOfMoves = lastMoves.Count;

                if (numberOfMoves > maxMoves)
                {
                    result.Clear();

                    maxMoves = numberOfMoves;
                    bestMove = move;

                    foreach (var m in lastMoves)
                    {
                        result.Push(_game.Field.Vertex(m.To));
                    }

                    result.Push(_game.Field.Vertex(bestMove));
                }
            }

            result.Push(_game.Field.Vertex(currentPosition));

            return result;
        }

        /* Iteration is human thing. Recursion divine */
        private Game MoveComputer(Point from, Point to, Stack<int> pathToGoal, Point goal)
        {
            var localGame = new Game(_game);
            localGame.Field.RemoveEdge(localGame.Field.Vertex(from), localGame.Field.Vertex(to));
            localGame.CurrentPosition = to;
            localGame.LastMoves.Add(new Move(localGame.PlayerTurn, to, from));

            //VictoryConditions(to);
            bool isTopGoal = to.Y < 0;
            bool isBottomGoal = to.Y > localGame.Height;

            if (goal.Y == to.Y)
            {
                localGame.IsGameOver = true;
            }

            if (localGame.IsGameOver)
            {
                localGame.NumberOfMoves++;
                return localGame;
            }

            if (localGame.Field.IsMoveIntoTheVoid(to))
            {
                return localGame;
            }
            else // bump and move further
            {
                int nextMove = pathToGoal.Pop();
                MoveComputer(localGame.CurrentPosition, localGame.Field.Position(nextMove), pathToGoal, goal);
            }

            return localGame;
        }
    }
}
