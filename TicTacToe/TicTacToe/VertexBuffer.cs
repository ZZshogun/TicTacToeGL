using System;
using OpenTK.Graphics.OpenGL;

namespace TicTacToe
{
    public sealed class VertexBuffer : IDisposable
    {
        public static readonly int MaxVertexCount = int.MaxValue;
        public static readonly int MinVertexCount = 1;

        private bool disposed;

        public readonly int VertexBufferHandle;
        public readonly VertexInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;

        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = false)
        {
            disposed = false;
            if (vertexCount < MinVertexCount || vertexCount > MaxVertexCount) 
                throw new ArgumentOutOfRangeException(nameof(vertexCount));

            VertexInfo = vertexInfo;
            VertexCount = vertexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StreamDraw;
            if (IsStatic) hint = BufferUsageHint.StaticDraw;

            int vertexSizeInBytes = Vertex.VertexInfo.SizeInBytes;

            VertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * vertexSizeInBytes, IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        ~VertexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferHandle);

            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
                throw new ArgumentException("SetData<T> : T and VertexInfo.Type is mismatched");
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            if (count <= 0 || count > VertexCount || count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
