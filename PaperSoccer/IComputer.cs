using System.Collections.Generic;
using System.Drawing;

namespace PaperSoccer
{
    interface IComputer
    {
        Stack<int> GetPathToGoal(Field field, Point currentPosition, Point goal);
    }
}
