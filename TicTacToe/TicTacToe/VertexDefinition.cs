using System;
using OpenTK.Mathematics;

namespace TicTacToe
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            Offset = offset;
        }
    }

    public sealed class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attributes)
        {
            Type = type;
            SizeInBytes = 0;
            VertexAttributes = attributes;
            
            foreach(var attribute in attributes)
                SizeInBytes += attribute.ComponentCount * sizeof(float);
        }

    }

    public struct Vertex
    {
        public Vector2 Position;
        public Color4 Color;
        public Vector2 TexCoord;

        public static readonly VertexInfo VertexInfo = new(
            typeof(Vertex),
            new("Position", 0, 2, 0),
            new("Color", 1, 4, 2 * sizeof(float)),
            new("TexCoord", 2, 2, 6 * sizeof(float))
            );

        public Vertex(Vector2 position, Color4 color, Vector2 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }
    }
}
