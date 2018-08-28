using System;

namespace PaperSoccer
{
    class Edge
    {
        public int From         { get; private set; }
        public int To           { get; private set; }
        public double Weight    { get; private set; }

        public Edge(int from, int to, double weight)
        {
            if (from < 0 || to < 0) throw new IndexOutOfRangeException("Vertex must be nonnegative integer");
            if (double.IsNaN(weight)) throw new ArgumentException("Weight is NaN");
            From = from;
            To = to;
            Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("{0} --{1:00.00}--> {2}", From, Weight, To);
        }
    }
}
