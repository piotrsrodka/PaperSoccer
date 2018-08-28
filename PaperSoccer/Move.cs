using System.Drawing;
using PaperSoccer.Enums;

namespace PaperSoccer
{
    public struct Move
    {
        public PlayerOrder PlayerOrder;
        public Point To;
        public Point From;

        public Move(PlayerOrder playerDescription, Point to, Point from)
        {
            PlayerOrder = playerDescription;
            To = to;
            From = from;
        }
    }
}