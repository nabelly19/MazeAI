namespace MazeAI;

public class Maze
{
    public Space?      Root   { get; private set; }
    public List<Space> Spaces { get; } = new();

    public void Reset()
    {
        foreach (var space in Spaces)
            space.Reset();
    }

    public static Maze Prim(int sx, int sy, bool nonTree = false)
    {
        const int horizontalSize = 48;
        const int verticalSize = 27;
        var maze = new Maze();
        var priority = new PriorityQueue<(int i, int j), byte>();
        var topGrid = new byte[horizontalSize, verticalSize];
        var rightGrid = new byte[horizontalSize, verticalSize];
        var vertices = new Space?[horizontalSize, verticalSize];

        for (int i = 0; i < horizontalSize; i++)
        {
            for (int j = 0; j < verticalSize; j++)
            {
                topGrid[i, j] = (byte)Random.Shared.Next(255);
                rightGrid[i, j] = (byte)Random.Shared.Next(255);
            }
        }

        maze.Root = Add(0, 0);

        while (priority.Count > 0)
        {
            var pos = priority.Dequeue();
            Connect(pos.i, pos.j);
        }

        for (int i = 0; i < horizontalSize; i++)
        {
            for (int j = 0; j < verticalSize; j++)
            {
                if (j > 0)
                {
                }

                if (j < 26)
                {
                }

                if (i > 0)
                {
                }

                if (i < 47)
                {
                }

                if ((i == horizontalSize / 2) && (j == verticalSize / 2))
                    maze.Root = vertices[i, j];
            }
        }

        return maze;

        Space? Add(int i, int j)
        {
            if (vertices[i, j] is null)
            {
                var newSpace = new Space
                {
                    X = i,
                    Y = j,
                    Exit = sx == i && sy == j
                };
                maze.Spaces.Add(newSpace);
                vertices[i, j] = newSpace;
            }

            var top = j == 0 || vertices[i, j - 1] is not null ? byte.MaxValue : topGrid[i, j];
            var bot = j == 26 || vertices[i, j + 1] is not null ? byte.MaxValue : topGrid[i, j + 1];
            var rig = i == 47 || vertices[i + 1, j] is not null ? byte.MaxValue : rightGrid[i, j];
            var lef = i == 0 || vertices[i - 1, j] is not null ? byte.MaxValue : rightGrid[i - 1, j];

            var min = byte.Min(
                byte.Min(top, bot),
                byte.Min(lef, rig)
            );

            if (min == byte.MaxValue)
                return vertices[i, j];

            priority.Enqueue((i, j), min);

            return vertices[i, j];
        }

        void Connect(int i, int j)
        {
            var crr = vertices[i, j];
            
            if (crr is null)
                return;

            var top = j == 0 || vertices[i, j - 1] is not null ? byte.MaxValue : topGrid[i, j];
            var bot = j == 26 || vertices[i, j + 1] is not null ? byte.MaxValue : topGrid[i, j + 1];
            var rig = i == 47 || vertices[i + 1, j] is not null ? byte.MaxValue : rightGrid[i, j];
            var lef = i == 0 || vertices[i - 1, j] is not null ? byte.MaxValue : rightGrid[i - 1, j];

            var min = byte.Min(
                byte.Min(top, bot),
                byte.Min(lef, rig)
            );

            if (min == byte.MaxValue)
                return;

            if (min == top || NonTreeCond())
            {
                var newSpace = Add(i, j - 1);
                crr.Top = newSpace;
                newSpace!.Bottom = crr;
            }

            if (min == lef || NonTreeCond())
            {
                var newSpace = Add(i - 1, j);
                crr.Left = newSpace;
                newSpace!.Right = crr;
            }

            if (min == rig || NonTreeCond())
            {
                var newSpace = Add(i + 1, j);
                crr.Right = newSpace;
                newSpace!.Left = crr;
            }

            if (min == bot || NonTreeCond())
            {
                var newSpace = Add(i, j + 1);
                crr.Bottom = newSpace;
                newSpace!.Top = crr;
            }

            Add(i, j);
            return;

            bool NonTreeCond() => nonTree && Random.Shared.NextSingle() < 0.1f && i is > 1 and < 46 && j is > 1 and < 25;
        }
    }
}