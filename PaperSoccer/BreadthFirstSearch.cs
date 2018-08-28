using System.Collections.Generic;

namespace PaperSoccer
{
    /// <summary>
    /// Algorithm from Algorithms 4 by Robert Sedgewick & Kevin Wayne
    /// </summary>
    class BreadthFirstSearch
    {
        private readonly bool[] _marked;      // marked[v] = is there an s-v path?
        private readonly int[] _edgeTo;       // edgeTo[v] = previous edge on shortest s-v path
        private readonly int[] _distanceTo;       // distTo[v] = number of edges shortest s-v path

        public BreadthFirstSearch(Graph g, int s)
        {
            _marked = new bool[g.GetVertices()];
            _distanceTo = new int[g.GetVertices()];
            _edgeTo = new int[g.GetVertices()];
            BfsSingleSource(g, s);
        }

        // breadth-first search from a single source
        private void BfsSingleSource(Graph g, int source)
        {
            var queue = new Queue<int>();

            for (int v = 0; v < g.GetVertices(); v++)
            {
                _distanceTo[v] = int.MaxValue;
            }

            _distanceTo[source] = 0;
            _marked[source] = true;
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();

                foreach (int w in g.GetAdjacencyList(v))
                {
                    if (!_marked[w])
                    {
                        _edgeTo[w] = v;
                        _distanceTo[w] = _distanceTo[v] + 1;
                        _marked[w] = true;
                        queue.Enqueue(w);
                    }
                }
            }
        }

        public bool HasPathTo(int v) { return _marked[v]; }

        public int DistTo(int v) { return _distanceTo[v]; }

        public Stack<int> PathTo(int v)
        {
            if (!HasPathTo(v))
            {
                return null;
            }

            var path = new Stack<int>();
            int x;

            for (x = v; _distanceTo[x] != 0; x = _edgeTo[x])
            {
                path.Push(x);
            }

            path.Push(x);
            return path;
        }
    }
}
