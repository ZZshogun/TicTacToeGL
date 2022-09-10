using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace TicTacToe
{
    public sealed class Shape : IDisposable
    {
        private bool _disposed = false;

        public static int ID = 1;
        private int objectID;
        private Vector2 _position = Vector2.Zero;
        private Vector2 _scale = Vector2.One;
        private Color4 _color = Color4.White;
        private TextureFileType _texfileType;
        private string _texture = "";
        private bool _tex_init = false;

        private VertexArray? VertexArray;
        private IndexBuffer? IndexBuffer;
        private VertexBuffer? VertexBuffer;
        private VertexTexture? VertexTexture;

        private Vertex[]? vertices;

        public Shape(Vector2 position = default, Vector2 scale = default, Color4 color = default, string texture = "", TextureFileType textype = TextureFileType.None)
        {
            if (scale == default) scale = Vector2.One;
            if (color == default) color = Color4.White;

            Position = position;
            Scale = scale;
            Color = color;
            Texture = texture;
            TextureFileType = textype;
            Setup();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            VertexArray?.Dispose();
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
            VertexTexture?.Dispose();

            GC.SuppressFinalize(this);
        }

        ~Shape() => Dispose();

        private void Setup()
        {
            objectID = ID++;
            float x = Position.X, y = Position.Y;
            float w = Scale.X, h = Scale.Y;

            vertices = new Vertex[]
            {
                new(new Vector2(x, y + h), _color, new Vector2(0, 1)),
                new(new Vector2(x + w, y + h), _color, new Vector2(1, 1)),
                new(new Vector2(x + w, y), _color, new Vector2(1, 0)),
                new(new Vector2(x, y), _color, new Vector2(0, 0)),
            };

            int[] indicies =
            {
                0, 1, 2,
                0, 2, 3,
            };

            VertexBuffer = new(Vertex.VertexInfo, vertices.Length, isStatic: true);
            VertexBuffer.SetData(vertices, vertices.Length);

            IndexBuffer = new(indicies.Length, isStatic: true);
            IndexBuffer.SetData(indicies, indicies.Length);

            VertexArray = new(VertexBuffer);

            VertexTexture = new();
            if (_tex_init) VertexTexture.SetData(_texture, _texfileType);

            Logger.Log(this);
        }

        public void Draw(ShaderProgram shaderProgram)
        {
            //Logger.Log(this);
            shaderProgram.SetUniform("fTexbool", _tex_init ? 0 : 1);
            if (_tex_init) VertexTexture?.Bind();

            GL.UseProgram(shaderProgram.ShaderProgramHandle);

            GL.BindVertexArray(VertexArray!.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer!.IndexBufferHandle);
            GL.DrawElements(BeginMode.Triangles, IndexBuffer.IndexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            if (_tex_init) VertexTexture?.Unbind();
        }

        public override string ToString()
        {
            return $"ID : {objectID}\nposition : {Position}\nscale : {Scale}\ncolor : {Color}\ntexture : {(Texture != "" ? Texture : "None")}";
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                if (vertices is null || VertexBuffer is null) return;
                vertices[0].Position = new(_position.X, _position.Y + _scale.Y);
                vertices[1].Position = new(_position.X + _scale.X, _position.Y + _scale.Y);
                vertices[2].Position = new(_position.X + _scale.X, _position.Y);
                vertices[3].Position = new(_position.X, _position.Y);
                VertexBuffer.SetData(vertices, vertices.Length);
            }
        }

        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (vertices is null || VertexBuffer is null) return;
                vertices[0].Position = new(_position.X, _position.Y + _scale.Y);
                vertices[1].Position = new(_position.X + _scale.X, _position.Y + _scale.Y);
                vertices[2].Position = new(_position.X + _scale.X, _position.Y);
                vertices[3].Position = new(_position.X, _position.Y);
                VertexBuffer.SetData(vertices, vertices.Length);
            }
        }

        public Color4 Color
        {
            get => _color;
            set
            {
                _color = value;
                if (vertices is null || VertexBuffer is null) return;
                for (int i = 0; i < vertices.Length; i++) vertices[i].Color = _color;
                VertexBuffer.SetData(vertices, vertices.Length);
            }
        }

        public string Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                if (_texture == "")
                {
                    _tex_init = false;
                    return;
                }
                _tex_init = true;
                VertexTexture?.SetData(_texture, TextureFileType);
            }
        }

        public TextureFileType TextureFileType
        {
            get => _texfileType;
            set => _texfileType = value;
        }
    }
}
