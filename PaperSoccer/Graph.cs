using System;
using System.Collections.Generic;
using System.Linq;

namespace PaperSoccer
{
    public class Graph
    {
        private readonly HashSet<int>[] _adjacencyList;

        public Graph(int vertices)
        {
            if (vertices < 0)
            {
                throw new Exception("Number of vertices must be nonnegative");
            }
            
            Vertices = vertices;
            Edges = 0;
            _adjacencyList = new HashSet<int>[vertices];
            
            for (int w = 0; w < vertices; w++)
            {
                _adjacencyList[w] = new HashSet<int>();
            }
        }

        public Graph(Graph graph)
        {
            Vertices = graph.Vertices;
            Edges = graph.Edges;

            for (int v = 0; v < Vertices; v++)
            {
                var reverse = new Stack<int>();
                
                foreach (var w in graph._adjacencyList[v])
                {
                    reverse.Push(w);
                }

                foreach (var w in reverse)
                {
                    _adjacencyList[v].Add(w);
                }
            }
        }

        public int Vertices { get; }

        public int Edges { get; private set; }

        public bool ExistEdge(int v, int w)
        {
            if (v < 0 || v >= Vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= Vertices) throw new IndexOutOfRangeException();

            return _adjacencyList[v].Contains(w);
        }

        public void AddEdge(int v, int w)
        {
            if (v < 0 || v >= Vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= Vertices) throw new IndexOutOfRangeException();

            _adjacencyList[v].Add(w);
            _adjacencyList[w].Add(v);
            Edges++;
        }

        public void RemoveEdge(int v, int w)
        {
            if (v < 0 || v >= Vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= Vertices) throw new IndexOutOfRangeException();

            if (_adjacencyList[v].Contains(w))
            {
                _adjacencyList[v].Remove(w);
            }

            if (_adjacencyList[w].Contains(v))
            {
                _adjacencyList[w].Remove(v);
            }

            Edges--;
        }

        public void RemoveAllEdges(int v)
        {
            if (v < 0 || v >= Vertices)
            {
                throw new IndexOutOfRangeException();
            }

            int w;

            while (DegreeOf(v) > 0)
            {
                w = _adjacencyList[v].First();
                RemoveEdge(v, w);
            }
        }

        public HashSet<int> GetAdjacencyList(int v) 
        {
            if (v < 0 || v >= Vertices)
            {
                throw new IndexOutOfRangeException();
            }

            return _adjacencyList[v]; 
        }

        public virtual int DegreeOf(int v) { return _adjacencyList[v].Count; }
    }
}
