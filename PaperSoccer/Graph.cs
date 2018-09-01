using System;
using System.Collections.Generic;
using System.Linq;

namespace PaperSoccer
{
    public class Graph
    {
        private readonly int _vertices;
        private int _edges;
        private readonly HashSet<int>[] _adjacencyList;

        public Graph(int vertices)
        {
            if (vertices < 0)
            {
                throw new Exception("Number of vertices must be nonnegative");
            }
            
            _vertices = vertices;
            _edges = 0;
            _adjacencyList = new HashSet<int>[vertices];
            
            for (int w = 0; w < vertices; w++)
            {
                _adjacencyList[w] = new HashSet<int>();
            }
        }

        public Graph(Graph graph)
        {
            _vertices = graph._vertices;
            _edges = graph._edges;

            for (int v = 0; v < _vertices; v++)
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

        public bool ExistEdge(int v, int w)
        {
            if (v < 0 || v >= _vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= _vertices) throw new IndexOutOfRangeException();

            return _adjacencyList[v].Contains(w);
        }

        public void AddEdge(int v, int w)
        {
            if (v < 0 || v >= _vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= _vertices) throw new IndexOutOfRangeException();

            _adjacencyList[v].Add(w);
            _adjacencyList[w].Add(v);
            _edges++;
        }

        public void RemoveEdge(int v, int w)
        {
            if (v < 0 || v >= _vertices) throw new IndexOutOfRangeException();
            if (w < 0 || w >= _vertices) throw new IndexOutOfRangeException();

            if (_adjacencyList[v].Contains(w))
            {
                _adjacencyList[v].Remove(w);
            }

            if (_adjacencyList[w].Contains(v))
            {
                _adjacencyList[w].Remove(v);
            }

            _edges--;
        }

        public void RemoveAllEdges(int v)
        {
            if (v < 0 || v >= _vertices)
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
            if (v < 0 || v >= _vertices)
            {
                throw new IndexOutOfRangeException();
            }

            return _adjacencyList[v]; 
        }

        public int GetVertices() { return _vertices; }

        public int GetEdges() { return _edges; }

        public virtual int DegreeOf(int v) { return _adjacencyList[v].Count; }
    }
}
