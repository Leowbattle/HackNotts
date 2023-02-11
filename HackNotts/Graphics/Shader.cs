using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackNotts.Graphics
{
    class ShaderCompilationException : Exception
    {
        public ShaderCompilationException(string? message) : base(message)
        {
        }
    }

    internal class Shader : IDisposable
    {
        static GL gl => Program.Instance.gl;

        public uint program;

        Shader(string vsSource, string fsSource)
        {
            program = LoadProgram(vsSource, fsSource);
        }

        static uint LoadProgram(string vsSource, string fsSource)
        {
            uint vs = LoadShader(vsSource, ShaderType.VertexShader);
            uint fs = LoadShader(fsSource, ShaderType.FragmentShader);

            uint program = gl.CreateProgram();
            gl.AttachShader(program, vs);
            gl.AttachShader(program, fs);
            gl.LinkProgram(program);
            gl.GetProgram(program, GLEnum.LinkStatus, out int status);
            if (status != (int)GLEnum.True)
            {
                throw new ShaderCompilationException(gl.GetProgramInfoLog(program));
            }

            gl.DetachShader(program, vs);
            gl.DetachShader(program, fs);
            gl.DeleteShader(vs);
            gl.DeleteShader(fs);

            return program;
        }

        static uint LoadShader(string source, ShaderType type)
        {
            uint shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);
            gl.GetShader(shader, GLEnum.CompileStatus, out int status);
            if (status != (int)GLEnum.True)
            {
                string log = gl.GetShaderInfoLog(shader);
                Console.WriteLine(log);
                throw new ShaderCompilationException(log);
            }

            return shader;
        }

        public static Shader FromFiles(string vsPath, string fsPath)
        {
            var s = new Shader(File.ReadAllText(vsPath), File.ReadAllText(fsPath));

            return s;
        }

        public void Dispose()
        {
            gl.DeleteProgram(program);
        }
    }
}
