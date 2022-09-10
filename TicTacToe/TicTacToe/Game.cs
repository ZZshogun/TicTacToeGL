using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TicTacToe.Properties;
using System.Reflection;

namespace TicTacToe
{
    internal class Game :  GameWindow
    {
        private Shape upperbarXturn;
        private Shape upperbarOturn;
        private Shape[]? lines;
        private Shape[,]? board;

        private Shape? tieScreen;
        private Shape? xWin;
        private Shape? oWin;

        private char[,] scorecheck;
        private string[] players;
        private bool turn = false;
        private bool end = false;
        private bool tie = false;
        private char winner = '_';
        private const char blank = '_';

        private ShaderProgram? shaderProgram;

        private Color4 bgcol = Color4.White;
        public Color4 BackgroundColor
        {
            get => bgcol;
            set
            {
                bgcol = value;
                GL.ClearColor(bgcol);
            }
        }

        public Game(int width, int height, string title = "Game", bool LogEnabled = true) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Title = title,
                Size = new Vector2i(width, height),
                StartVisible = false,
                StartFocused = true,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3, 3),
                WindowBorder = WindowBorder.Fixed,
            }
           )
        {
            CenterWindow();
            VSync = VSyncMode.Adaptive;

            Logger.LogEnabled = LogEnabled;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            shaderProgram?.SetUniform2("viewportSize", e.Size);

            OnRenderFrame(new());
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            IsVisible = true;

            GL.ClearColor(BackgroundColor);

            string vertexShaderCode = CustomFilestream.ReadFromResources(Resources.VertexShader);
            string fragmentShaderCode = CustomFilestream.ReadFromResources(Resources.FragmentShader);

            shaderProgram = new(vertexShaderCode, fragmentShaderCode);
            shaderProgram?.SetUniform2("viewportSize", new Vector2(Size.X, Size.Y));

            scorecheck = new char[3, 3];
            scorecheck.ForEach((i, j) => scorecheck[i, j] = blank);

            board = new Shape[3, 3];
            board.ForEach((i, j) =>
                board[i, j] = new(
                    position: new Vector2(i * 300 + 2, j * 300 + 2),
                    scale: new Vector2(300, 300),
                    textype: TextureFileType.BMP
                )
            );

            lines = new Shape[4];
            lines[0] = new(
                position: new Vector2(300, 0),
                scale: new Vector2(10, 900),
                color: Color4.Black
            );

            lines[1] = new(
                position: new Vector2(600, 0),
                scale: new Vector2(10, 900),
                color: Color4.Black
            );

            lines[2] = new(
                position: new Vector2(0, 300),
                scale: new Vector2(900, 10),
                color: Color4.Black
            );

            lines[3] = new(
                position: new Vector2(0, 600),
                scale: new Vector2(900, 10),
                color: Color4.Black
            );

            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            players = new string[2]
            {
                $"{path}\\Textures\\x.bmp",
                $"{path}\\Textures\\o.bmp"
            };

            upperbarOturn = new(
                position: new Vector2(0, 900),
                scale: new Vector2(900, 100),
                texture: $"{path}\\Textures\\upperbarO.bmp",
                textype: TextureFileType.BMP
            );

            upperbarXturn = new(
                position: new Vector2(0, 900),
                scale: new Vector2(900, 100),
                texture: $"{path}\\Textures\\upperbarX.bmp",
                textype: TextureFileType.BMP
            );

            tieScreen = new(
                position: new Vector2(0, 900),
                scale: new Vector2(900, 100),
                texture: $"{path}\\Textures\\tie.bmp",
                textype: TextureFileType.BMP
            );

            xWin = new(
                position: new Vector2(0, 900),
                scale: new Vector2(900, 100),
                texture: $"{path}\\Textures\\xwin.bmp",
                textype: TextureFileType.BMP
            );
            oWin = new(
                position: new Vector2(0, 900),
                scale: new Vector2(900, 100),
                texture: $"{path}\\Textures\\owin.bmp",
                textype: TextureFileType.BMP
            );

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            lines?.ForEach((i) => lines[i]?.Dispose());
            board?.ForEach((i, j) => board[i, j]?.Dispose());
            shaderProgram?.Dispose();
            
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (end && IsKeyDown(Keys.R))
            {
                end = false;
                tie = false;
                winner = blank;
                turn = winner == 'x';
                board?.ForEach((i, j) => board[i, j].Texture = "");
                scorecheck.ForEach((i, j) => scorecheck[i, j] = blank);
            }

            if (IsMouseButtonDown(MouseButton.Left) && board is not null && !end && MousePosition.Y > 100)
            {
                Vector2i pos = ((MousePosition - new Vector2(0, 100)) / 301).Floor();
                int newY = Math.Abs(pos.Y - 2);
                if (board[pos.X, newY].Texture == "")
                {
                    board[pos.X, newY].Texture = players[turn.ToInt()];
                    scorecheck[pos.X, newY] = turn ? 'o' : 'x';
                    tie = TieCondition();
                    end = WinCondition();
                    if (!end && tie) end = true;
                    else if (end && tie) tie = false;

                    if (end && !tie)
                        winner = turn ? 'o' : 'x';

                    turn = !turn;
                }
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (end)
            {
                if (tie)
                    tieScreen?.Draw(shaderProgram!);
                else if (winner == 'x')
                    xWin?.Draw(shaderProgram!);
                else
                    oWin?.Draw(shaderProgram!);
            }
            else
            {
                if (turn)
                    upperbarOturn?.Draw(shaderProgram!);
                else
                    upperbarXturn?.Draw(shaderProgram!);
            }
            
            board?.ForEach((i, j) => board[i, j]?.Draw(shaderProgram!));
            lines?.ForEach((i) => lines[i]?.Draw(shaderProgram!));

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        private bool WinCondition()
        {
            char cur = turn ? 'o' : 'x';
            for (int i = 0; i < 3; i++)
            {
                if (cur == scorecheck[i, 0] && scorecheck[i, 0] == scorecheck[i, 1] && scorecheck[i, 0] == scorecheck[i, 2])
                {
                    Logger.Log($"{cur} Wins!");
                    return true;
                }
                if (cur == scorecheck[0, i] && scorecheck[0, i] == scorecheck[1, i] && scorecheck[0, i] == scorecheck[2, i])
                {
                    Logger.Log($"{cur} Wins!");
                    return true;
                }
            }
            if (cur == scorecheck[0, 0] && scorecheck[0, 0] == scorecheck[1, 1] && scorecheck[0, 0] == scorecheck[2, 2])
            {
                Logger.Log($"{cur} Wins!");
                return true;
            }
            if (cur == scorecheck[0, 2] && scorecheck[0, 2] == scorecheck[1, 1] && scorecheck[0, 2] == scorecheck[2, 0])
            {
                Logger.Log($"{cur} Wins!");
                return true;
            }
            return false;
        }

        private bool TieCondition()
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (scorecheck[i, j] == blank) 
                        return false;
                }
            }

            Logger.Log("Tie!");
            return true;
        }
    }
}
