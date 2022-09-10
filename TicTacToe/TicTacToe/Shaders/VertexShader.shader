#version 330 core

uniform vec2 viewportSize;

layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec4 aColor;
layout(location = 2) in vec2 aTexCoord;

out vec4 Color;
out vec2 TexCoord;

void main()
{
    float nx = (aPosition.x / viewportSize.x) * 2 - 1;
    float ny = (aPosition.y / viewportSize.y) * 2 - 1;

    TexCoord = aTexCoord;
    Color = aColor;
    gl_Position = vec4(nx, ny, 0, 1);
}