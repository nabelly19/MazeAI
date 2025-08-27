using Pamella;

namespace MazeAI;

public class MazeView : Pamella.View
{
    private          bool   _help        = false;
    private          bool   _nonTreeMode = false;
    private          int    _solX        = 20;
    private          int    _solY        = 20;
    private          bool   _update      = false;
    private          bool   _solve       = false;
    private readonly Solver _solver      = new Solver();
    public          Maze   Maze { get; set; } = null!;

    protected override void OnStart(IGraphics g)
    {
        Maze = Maze.Prim(_solX, _solY, _nonTreeMode);
        _solver.Maze = Maze;
        g.SubscribeKeyDownEvent(key =>
        {
            switch (key)
            {
                case Input.Escape:
                    App.Close();
                    break;
                case Input.Space:
                    Maze = Maze.Prim(_solX, _solY, _nonTreeMode);
                    _solver.Maze = Maze;
                    Invalidate();
                    break;
                case Input.T:
                    _nonTreeMode = !_nonTreeMode;
                    Maze = Maze.Prim(_solX, _solY, _nonTreeMode);
                    _solver.Maze = this.Maze;
                    Invalidate();
                    break;
                case Input.H:
                    _help = !_help;
                    Invalidate();
                    break;
                case Input.S:
                    _solve = !_solve;
                    if (!_solve)
                    {
                        Maze.Reset();
                        Invalidate();
                    }

                    break;
                case Input.U:
                    _update = !_update;
                    break;
                case Input.A:
                    Maze.Reset();
                    _solver.Option++;
                    break;
            }
        });
    }

    protected override void OnRender(IGraphics g)
    {
        int y = 40;
        if (_help)
        {
            g.Clear(Color.White);
            Write($"Algoritmo Atual: {_solver.Algorithm}.");
            Write("h - Abrir/Fechar tela de ajuda.");
            Write("Esc - Fechar aplicação.");
            Write("Space - Regerar labirinto.");
            Write("S - Ligar/Desligar resolução do labirinto.");
            Write("U - Iniciar/Desligar atualização da saída.");
            Write("A - Mudar algorítimo.");
            Write("T - Iniciar/Desligar modo não-árvore.");
            return;
        }

        g.Clear(Color.Black);
        foreach (var space in Maze.Spaces)
            DrawSpace(space, g);

        return;

        void Write(string text)
        {
            g.DrawText(
                new RectangleF(0, y, g.Width, 40),
                Brushes.Red,
                text
            );
            y += 40;
        }
    }

    protected override void OnFrame(IGraphics g)
    {
        if (_update)
        {
            var dx = g.Width / 48;
            var dy = g.Height / 27;
            _solX = (int)(g.Cursor.X / dx);
            _solY = (int)(g.Cursor.Y / dy);

            foreach (var space in Maze.Spaces)
            {
                space.Exit = space.X == _solX && space.Y == _solY;
            }

            Invalidate();
        }

        if (!_solve) return;
        Maze.Reset();
        _solver.Solve();

        Invalidate();
    }

    private void DrawSpace(Space? space, IGraphics g)
    {
        if (space is null)
            return;

        var dx = g.Width / 48;
        var dy = g.Height / 27;
        var x = space.X * dx;
        var y = space.Y * dy;
        g.FillRectangle(x, y, dx, dy,
                        space == Maze.Root ? Brushes.Yellow :
                        space.Exit         ? Brushes.Green :
                        space.Visited      ? Brushes.Blue : Brushes.White
        );

        if (space.IsSolution)
        {
            Connect(space, space.Top, g);
            Connect(space, space.Left, g);
            Connect(space, space.Right, g);
            Connect(space, space.Bottom, g);
        }

        if (space?.Left is null)
            g.FillRectangle(x, y, 5, dy, Brushes.Black);
        if (space?.Top is null)
            g.FillRectangle(x, y, dx, 5, Brushes.Black);
    }

    private static void Connect(Space? a, Space? b, IGraphics g)
    {
        if (a is null || b is null)
            return;

        if (!a.IsSolution || !b.IsSolution)
            return;

        var dx = g.Width / 48;
        var dy = g.Height / 27;

        var x1 = a.X * dx + dx / 2;
        var y1 = a.Y * dy + dy / 2;

        var x2 = b.X * dx + dx / 2;
        var y2 = b.Y * dy + dy / 2;

        var width = x2 - x1;
        var height = y2 - y1;

        if (width < 0)
            width = -width;
        if (height < 0)
            height = -height;

        var x = 0;
        var y = 0;

        if (width == 0)
        {
            width = 12;
            x = x1 - 6;
            if (y1 < y2)
            {
                y = y1 - 3;
                height += 6;
            }
            else
            {
                y = y2 - 3;
                height += 6;
            }
        }

        if (height == 0)
        {
            height = 12;
            y = y1 - 6;
            if (x1 < x2)
            {
                x = x1 - 3;
                width += 6;
            }
            else
            {
                x = x2 - 3;
                width += 6;
            }
        }

        g.FillRectangle(x, y, width, height, Brushes.Red);
    }
}