using System.Collections.Generic;
using UnityEngine;

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


    static int width;
    static int height;

    static List<Vector2Int>[,] vertexes;

    static bool[,] visited;
    static Vector2Int[,] edgeTo;

    static void CreateMap(IMapData mapData)
    {
        width = mapData.Width;
        height = mapData.Height;

        vertexes = new List<Vector2Int>[width, height];

        visited = new bool[width, height];
        edgeTo = new Vector2Int[width, height];

        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                vertexes[i, j] = new List<Vector2Int>();
            }
        }


        for (int y = height - 1; y >= 0; y--)
        {
            for(int x = width - 1;x >= 0; x--)
            {
                Debug.Log($"{mapData.HasHorizontalWall(x, y)} : {mapData.HasVerticalWall(x, y)}, ({x},{y})");

                if (x - 1 >= 0 && !mapData.HasVerticalWall(x, y)) AddEdge(new Vector2Int(x, y), new Vector2Int(x - 1, y));
                if (y - 1 >= 0 && !mapData.HasHorizontalWall(x, y)) AddEdge(new Vector2Int(x, y), new Vector2Int(x, y - 1));
            }
        }

    }

    static void AddEdge(Vector2Int from, Vector2Int to)
    {
        vertexes[from.x, from.y].Add(to);
        vertexes[to.x, to.y].Add(from);
    }

    static List<Vector2Int> Adj(Vector2Int vertex)
    {
        return vertexes[vertex.x, vertex.y];
    }


    static void bfs(Vector2Int v)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(v);
        visited[v.x, v.y] = true;

        while (q.Count > 0)
        {
            Vector2Int w = q.Dequeue();
            foreach (Vector2Int i in Adj(w))
            {
                if (!visited[i.x, i.y])
                {
                    q.Enqueue(i);
                    visited[i.x, i.y] = true;
                    edgeTo[i.x, i.y] = w;
                }
            }
        }
    }


    public static List<Vector2Int> FindShortestPath(Vector2Int start, Vector2Int goal, IMapData mapData)
    {
        // TODO: Implement your pathfinding algorithm here
        CreateMap(mapData);

        bfs(start);

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