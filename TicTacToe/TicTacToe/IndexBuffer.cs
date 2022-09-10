using System;
using OpenTK.Graphics.OpenGL;

namespace TicTacToe
{
    public sealed class IndexBuffer : IDisposable
    {
        public static readonly int MaxIndexCount = int.MaxValue;
        public static readonly int MinIndexCount = 1;

        private bool _disposed = false;

        public readonly int IndexBufferHandle;
        public readonly int IndexCount;

        public IndexBuffer(int indexCount, bool isStatic = false)
        {
            if (indexCount < MinIndexCount || indexCount > MaxIndexCount)
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            
            IndexCount = indexCount;

            BufferUsageHint hint = BufferUsageHint.StreamDraw;
            if (isStatic) hint = BufferUsageHint.StaticDraw;

            IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(IndexBufferHandle);

            GC.SuppressFinalize(this);
        }

        ~IndexBuffer()
        {
            Dispose();
        }

        public void SetData(int[] data, int count)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (count <= 0 || count > IndexCount || count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
