using System;
using OpenTK.Graphics.OpenGL;

namespace TicTacToe
{
    public sealed class VertexArray : IDisposable
    {
        private bool disposed = false;

        public VertexBuffer VertexBuffer;
        public int VertexArrayHandle;

        public VertexArray(VertexBuffer vertexBuffer)
        {
            if (vertexBuffer is null)
                throw new ArgumentNullException(nameof(vertexBuffer));

            VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = VertexBuffer.VertexInfo.SizeInBytes;

            VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferHandle);

            foreach (var attr in Vertex.VertexInfo.VertexAttributes)
            {
                GL.VertexAttribPointer(attr.Index, attr.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attr.Offset);
                GL.EnableVertexAttribArray(attr.Index);
            }

            GL.BindVertexArray(0);
        }

        ~VertexArray()
        {
            Dispose();
        }

        public void Dispose() 
        {
            if (disposed) return;
            disposed = true;

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VertexArrayHandle);

            GC.SuppressFinalize(this);
        }
    }
}
