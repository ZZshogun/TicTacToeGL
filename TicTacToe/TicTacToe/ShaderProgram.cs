using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace TicTacToe
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        private bool _disposed = false;

        public readonly int ShaderProgramHandle;
        public readonly int VertexShaderHandle;
        public readonly int FragmentShaderHandle;

        private readonly ShaderUniform[] ShaderUniforms;
        private readonly ShaderAttribute[] ShaderAttributes;

        public ShaderProgram(string vertexShaderCode, string fragmentShaderCode)
        {
            if (!CompileVertexShader(vertexShaderCode, out VertexShaderHandle, out string vertexErrMsg))
                throw new ArgumentException($"Vertex Shader : {vertexErrMsg}");

            if (!CompileFragmentShader(fragmentShaderCode, out FragmentShaderHandle, out string fragmentErrMsg))
                throw new ArgumentException($"Fragment Shader : {fragmentErrMsg}");

            ShaderProgramHandle = CreateAndLinkProgram(VertexShaderHandle, FragmentShaderHandle);

            ShaderUniforms = CreateUniformList(ShaderProgramHandle);
            ShaderAttributes = CreateAttribList(ShaderProgramHandle);
        }

        ~ShaderProgram()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(FragmentShaderHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramHandle);

            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniforms()
        {
            ShaderUniform[] result = new ShaderUniform[ShaderUniforms.Length];
            Array.Copy(ShaderUniforms, result, ShaderUniforms.Length);
            return result;
        }
        public ShaderAttribute[] GetAttributes()
        {
            ShaderAttribute[] result = new ShaderAttribute[ShaderAttributes.Length];
            Array.Copy(ShaderAttributes, result, ShaderAttributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
                throw new ArgumentException(nameof(name));
            if (uniform.Type != ActiveUniformType.Float && uniform.Type != ActiveUniformType.Int)
                throw new ArgumentException(nameof(uniform.Type));

            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }

        public void SetUniform2(string name, Vector2 vec)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
                throw new ArgumentException(nameof(name));
            if (uniform.Type != ActiveUniformType.FloatVec2 && uniform.Type != ActiveUniformType.IntVec2)
                throw new ArgumentException(nameof(uniform.Type));

            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform2(uniform.Location, vec);
            GL.UseProgram(0);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new();

            for(int i = 0; i < ShaderUniforms.Length; i++)
            {
                uniform = ShaderUniforms[i];
                if (name == uniform.Name) return true;
            }
            return false;
        }

        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
            if (vertexShaderInfo != string.Empty)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static bool CompileFragmentShader(string fragmentShaderCode, out int fragmentShaderHandle, out string errorMessage)
        {
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

            string fragmentShaderInfo = GL.GetShaderInfoLog(fragmentShaderHandle);
            if (fragmentShaderInfo != String.Empty)
            {
                errorMessage = fragmentShaderInfo;
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static int CreateAndLinkProgram(int vertexShaderHandle, int fragmentShaderHandle)
        {
            int shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);

            return shaderProgramHandle;
        }

        public static ShaderUniform[] CreateUniformList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            
            ShaderUniform[] uniformList = new ShaderUniform[uniformCount];

            Logger.AddFlush("Uniforms :");
            for(int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(shaderProgramHandle, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int location = GL.GetUniformLocation(shaderProgramHandle, name);

                Logger.AddFlush($"{type} : {name} @ {location}");

                uniformList[i] = new(name, location, type);
            }

            Logger.Flush();

            return uniformList;
        }

        public static ShaderAttribute[] CreateAttribList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attribCount);

            ShaderAttribute[] attribList = new ShaderAttribute[attribCount];

            Logger.AddFlush("Attributes :");
            for (int i = 0; i < attribCount; i++)
            {
                GL.GetActiveAttrib(shaderProgramHandle, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int location = GL.GetAttribLocation(shaderProgramHandle, name);

                Logger.AddFlush($"{type} : {name} @ {location}");

                attribList[i] = new(name, location, type);
            }

            Logger.Flush();

            return attribList;
        }
    }
}
