using System.Collections.Generic;
using System.Drawing;

namespace PaperSoccer
{
    /// <summary>
    /// Walter is very simple computer that can find shortest path to goal.
    /// Can be considered as 'Easy' computer opponent
    /// </summary>
    class Walter : IComputer
    {
        public Stack<int> GetPathToGoal(Field field, Point currentPosition, Point goal)
        {
            var bfs = new BreadthFirstSearch(field, field.Vertex(currentPosition));
            return bfs.PathTo(field.Vertex(goal));
        }
    }
}
