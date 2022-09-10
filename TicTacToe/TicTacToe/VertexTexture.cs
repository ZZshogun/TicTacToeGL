using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace TicTacToe
{
    public enum TextureFileType
    {
        None,
        BMP,
    }

    public sealed class VertexTexture : IDisposable
    {
        private bool _disposed = false;

        public readonly int TextureHandler;

        public VertexTexture()
        {
            TextureHandler = GL.GenTexture();
        }

        public void SetData(string texturePath, TextureFileType type, int level = 0, int border = 0)
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureHandler);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            switch (type)
            {
                case TextureFileType.None:
                    break;
                case TextureFileType.BMP:
                    BitmapData texData = CustomFilestream.LoadImageBMP(texturePath);
                    GL.TexImage2D(
                        TextureTarget.Texture2D,
                        level,
                        PixelInternalFormat.Rgb,
                        texData.Width,
                        texData.Height,
                        border,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                        PixelType.UnsignedByte,
                        texData.Scan0
                    );
                    break;
                default:
                    Logger.LogError($"{type} is not a valid texture type.");
                    break;
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureHandler);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTexture(TextureHandler);

            GC.SuppressFinalize(this);
        }

        ~VertexTexture()
        {
            if (!_disposed) Dispose();
        }
    }
}
