using Microsoft.VisualBasic.ApplicationServices;

namespace MazeAI;

public class Solver
{
    public int Option { get; set; }
    public Maze Maze { get; set; } = null!;

    public string Algorithm
    {
        get
        {
            return (Option % 4) switch
            {
                0 => "DFS",
                1 => "BFS",
                2 => "dijkstra",
                _ => "aStar"
            };
        }
    }

    public void Solve()
    {
        var goal = Maze.Spaces.FirstOrDefault(s => s.Exit);

        if (Maze.Root is null || goal is null)
            return;

        switch (Option % 4)
        {
            case 0:
                DFS(Maze.Root, goal);
                break;
            case 1:
                BFS(Maze.Root, goal);
                break;
            case 2:
                Dijkstra(Maze.Root, goal);
                break;
            case 3:
                AStar(Maze.Root, goal);
                break;
        }
    }

    private static bool DFS(Space start, Space goal)
    {
        var crrSpace = start;
        var spaceStack = new Stack<Space>();
        crrSpace.Visited = true;

        while (crrSpace != goal)
        {
            if (crrSpace.Right is not null && !crrSpace.Right.Visited)
            {
                crrSpace.Right.Visited = true;
                spaceStack.Push(crrSpace);
                crrSpace = crrSpace.Right;
                continue;
            }
            if (crrSpace.Left is not null && !crrSpace.Left.Visited)
            {
                crrSpace.Left.Visited = true;
                spaceStack.Push(crrSpace);
                crrSpace = crrSpace.Left;
                continue;
            }
            if (crrSpace.Top is not null && !crrSpace.Top.Visited)
            {
                crrSpace.Top.Visited = true;
                spaceStack.Push(crrSpace);
                crrSpace = crrSpace.Top;
                continue;
            }
            if (crrSpace.Bottom is not null && !crrSpace.Bottom.Visited)
            {
                crrSpace.Bottom.Visited = true;
                spaceStack.Push(crrSpace);
                crrSpace = crrSpace.Bottom;
                continue;
            }
            crrSpace = spaceStack.Pop();
        }

        foreach (var solution in spaceStack)
        {
            solution.IsSolution = true;
        }
        start.IsSolution = true;
        goal.IsSolution = true;
        return true;
    }

    private static bool BFS(Space start, Space goal)
    {
        var prev = new Dictionary<Space, Space>();
        var queue = new Queue<Space>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var currSpace = queue.Dequeue();

            if (currSpace.Visited)
                continue;

            currSpace.Visited = true;

            if (currSpace == goal)
            {
                var attempt = goal;
                while(attempt != start )
                {
                    attempt.IsSolution = true;
                    if(!prev.ContainsKey(attempt))
                        return false;
                    attempt = prev[attempt];    
                }
                start.IsSolution = true;
                return true;
            }

            foreach (var solSpace in currSpace.Neighbours())
            {
                if (solSpace is not null && !solSpace.Visited)
                {
                    if(!prev.ContainsKey(solSpace))
                        prev[solSpace] = currSpace;
                    queue.Enqueue(solSpace);
                }
            }
        }
        return false;
    }

    private static bool Dijkstra(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0.0f);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var crrSpace = queue.Dequeue();
            crrSpace.Visited = true;

            if (crrSpace == goal)
                break;

            foreach (var space in crrSpace.Neighbours())
            {
                if (space is null)
                    continue;

                var newWeight = dist[crrSpace] + 1;

                if (!dist.ContainsKey(space))
                {
                    dist[space] = float.PositiveInfinity;
                    prev[space] = null!;
                }

                if (newWeight < dist[space])
                {
                    dist[space] = newWeight;
                    prev[space] = crrSpace;
                    queue.Enqueue(space, newWeight);
                }
            }
        }

        var attempt = goal;

        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt.IsSolution = true;
            attempt = prev[attempt];
        }
        attempt.IsSolution = true;

        return true;
    }

    private static bool AStar(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0.0f);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var crrSpace = queue.Dequeue();
            crrSpace.Visited = true;

            if (crrSpace == goal)
                break;

            foreach (var space in crrSpace.Neighbours())
            {
                if (space is null)
                    continue;

                if (!dist.ContainsKey(space))
                {
                    dist[space] = float.PositiveInfinity;
                    prev[space] = null!;
                }

                var dx = space.X - goal.X;
                var dy = space.Y - goal.Y;

                var penalty = MathF.Sqrt(dx * dx + dy * dy);
                var newWeight = dist[crrSpace] + 1 + penalty;


                if (newWeight < dist[space])
                {
                    dist[space] = newWeight;
                    prev[space] = crrSpace;
                    queue.Enqueue(space, newWeight + penalty);
                }
            }
        }

        var attempt = goal;

        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt.IsSolution = true;
            attempt = prev[attempt];
        }
        attempt.IsSolution = true;

        return true;
    }
}