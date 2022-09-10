#version 330 core

in vec4 Color;
in vec2 TexCoord;
out vec4 fColor;

uniform float fTexbool;
uniform sampler2D fTexture;

void main()
{
	vec4 tex = texture(fTexture, TexCoord);
	fColor = Color * (tex + vec4(fTexbool));
}