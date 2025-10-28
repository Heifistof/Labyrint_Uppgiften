using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public static class PathfindingAlgorithm
{
    /* <summary>
     TODO: Implement pathfinding algorithm here
     Find the shortest path from start to goal position in the maze.
     
     Dijkstra's Algorithm Steps:
     1. Initialize distances to all nodes as infinity
     2. Set distance to start node as 0
     3. Add start node to priority queue
     4. While priority queue is not empty:
        a. Remove node with minimum distance
        b. If it's the goal, reconstruct path
        c. For each neighbor:
           - Calculate new distance through current node
           - If shorter, update distance and add to queue
     
     MAZE FEATURES TO HANDLE:
     - Basic movement cost: 1.0 between adjacent cells
     - Walls: Some have infinite cost (impassable), others have climbing cost
     - Vents (teleportation): Allow instant travel between distant cells with usage cost
     
     AVAILABLE DATA STRUCTURES:
     - Dictionary<Vector2Int, float> - for tracking distances
     - Dictionary<Vector2Int, Vector2Int> - for tracking previous nodes (path reconstruction)
     - SortedSet<T> or List<T> - for priority queue implementation
     - mapData provides methods to check walls, vents, and boundaries
     
     HINT: Start simple with BFS (ignore wall costs and vents), then extend to weighted Dijkstra
     </summary> */

    static float ventCost = 10f;

    struct Edge
    {
        public Vector2Int destination;
        public float weight;
    }

    

    static int width;
    static int height;

    static List<Edge>[,] edges;

    static bool[,] visited;
    static Vector2Int[,] edgeTo;

    static float[,] distTo;


    static void CreateMap(IMapData mapData)
    {
        width = mapData.Width;
        height = mapData.Height;

        edges = new List<Edge>[width, height];

        visited = new bool[width, height];
        edgeTo = new Vector2Int[width, height];
        distTo = new float[width, height];

        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                edges[i, j] = new List<Edge>();
            }
        }


        for (int y = height - 1; y >= 0; y--)
        {
            for(int x = width - 1;x >= 0; x--)
            {

                AddEdge(x, y, mapData);
            }
        }

        foreach (Vector2Int from in mapData.GetAllVentPositions())
        {
            foreach (Vector2Int to in mapData.GetOtherVentPositions(from))
            {
                AddOneWayEdge(from, to, ventCost);
            }
        }

    }

    static void AddEdge(int x, int y, IMapData mapData)
    {
        if (x - 1 >= 0 && !mapData.HasVerticalWall(x, y)) // no vertical wall
        {
            AddEdge(new Vector2Int(x, y), new Vector2Int(x-1, y), 1);
        }
        else if (x - 1 >= 0)
        {
            float w = mapData.GetVerticalWallCost(x, y);
            AddEdge(new Vector2Int(x, y), new Vector2Int(x - 1, y), w);
        }


        if (y - 1 >= 0 && !mapData.HasHorizontalWall(x, y)) // no horizontal wall
        {
            AddEdge(new Vector2Int(x, y), new Vector2Int(x, y - 1), 1);
        } else if (y - 1 >= 0)
        {
            float w = mapData.GetHorizontalWallCost(x, y);
            AddEdge(new Vector2Int(x, y), new Vector2Int(x, y - 1), w);
        }

    }

    static void AddEdge(Vector2Int from, Vector2Int to, float weight)
    {
        edges[from.x, from.y].Add(new Edge { destination = to, weight = weight });
        edges[to.x, to.y].Add(new Edge { destination = from, weight = weight });
    }

    static void AddOneWayEdge(Vector2Int from, Vector2Int to, float weight)
    {
        edges[from.x, from.y].Add(new Edge { destination = to, weight = weight });
    }

    static List<Edge> Adj(Vector2Int vertex)
    {
        
        return edges[vertex.x, vertex.y];
    }


    /// <summary>
    /// Bredden först sökning, rekursivt tar nästa del
    /// </summary>
    /// <param name="v"></param>
    static void bfs(Vector2Int v)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(v);
        visited[v.x, v.y] = true;

        while (q.Count > 0)
        {
            Vector2Int w = q.Dequeue();
            foreach (Edge e in Adj(w))
            {
                if (e.weight > 1) // ignorera alla vägar där det finns vägg
                {
                    continue;
                }
                Vector2Int i = e.destination;
                if (!visited[i.x, i.y])
                {
                    q.Enqueue(i);
                    visited[i.x, i.y] = true;
                    edgeTo[i.x, i.y] = w;
                }
            }
        }
    }

    /// <summary>
    /// Dijkstras algoirtm
    /// </summary>
    /// <param name="start"></param>
    /// 
    static List<DijPrioHolder> prio; 
   
    static void Dijkstras(Vector2Int start)
    {
        // set all nodes to infinite distance
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                distTo[x, y] = float.PositiveInfinity;
            }
        }

        distTo[start.x, start.y] = 0;


        prio = new List<DijPrioHolder>();

        prio.Add(new DijPrioHolder {vertex = start, cost = 0});

        while (prio.Count > 0)
        {
            Vector2Int v = prio.ElementAt(0).vertex;
            prio.RemoveAt(0);
            visited[v.x, v.y] = true;
            RelaxEdge(v);
        }



    }

    static void RelaxEdge(Vector2Int v)
    {
        foreach(Edge e in Adj(v))
        {
            Vector2Int w = e.destination;
            if (distTo[w.x, w.y] > (distTo[v.x, v.y] + e.weight))
            {
                distTo[w.x, w.y] = (distTo[v.x, v.y] + e.weight);
                edgeTo[w.x, w.y] = v;

                if (prio.Contains(new DijPrioHolder() {vertex = w}))
                {
                    
                    prio.Remove(new DijPrioHolder() { vertex = w });
                    prio.Add(new DijPrioHolder() { vertex = w, cost = distTo[w.x, w.y]});
                    prio.Sort();
                } else
                {
                    prio.Add(new DijPrioHolder() { vertex = w, cost = distTo[w.x, w.y] });
                    prio.Sort();
                }
            }
        }
    }


    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        // create graph
        CreateMap(mapData);


        Dijkstras(start);

        Stack<Vector2Int> tempStack = new Stack<Vector2Int>();

        tempStack.Push(goal);
        Vector2Int current = goal;

        
        
        while (current != start && visited[goal.x, goal.y])
        {
            
            current = edgeTo[current.x, current.y];
            tempStack.Push(current);
        }


        List<Vector2Int> tempList = new List<Vector2Int>();

        while (tempStack.Count > 0)
        {
            tempList.Add(tempStack.Pop());
        }

        return tempList;

        Debug.LogWarning("FindShortestPath not implemented yet!");
        return null;
    }

    public static bool IsMovementBlocked(Vector2Int from, Vector2Int to, IMapData mapData)
    {
        // TODO: Implement movement blocking logic
        // For now, allow all movement so character can move while you work on pathfinding
        return false;
    }
}