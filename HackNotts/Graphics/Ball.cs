using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackNotts.Graphics
{
	internal class Ball
	{
		static GL gl => Program.Instance.gl;

		static Shader shader;

		static uint vao;
		static uint vbo_pos;

		static int mvpLoc;
		static int modelLoc;
		static int camPosLoc;

		public class BallData
		{
			public vec3 Pos;
		}

		public static List<BallData> Balls = new List<BallData>();

		public static void Draw()
		{
			mat4 modelMat = Matrix4X4.CreateRotationY(Program.Instance.GameTime * Util.Deg2Rad(90));
			mat4 mvp = modelMat * Program.Instance.viewMat * Program.Instance.projMat;

			gl.BindVertexArray(vao);

			gl.UseProgram(shader.program);

			unsafe
			{
				gl.UniformMatrix4(mvpLoc, 1, false, (float*)&mvp);
				gl.UniformMatrix4(modelLoc, 1, false, (float*)&modelMat);

				vec3 camPos = Program.Instance.CamPos;
				gl.Uniform3(camPosLoc, camPos.X, camPos.Y, camPos.Z);
			}

			gl.DrawArrays(GLEnum.Triangles, 0, 36);

			Balls.Clear();
		}

		public static void InitResources()
		{
			unsafe
			{
				shader = Shader.FromFiles("Assets/Shaders/ball.vert", "Assets/Shaders/ball.frag");
				mvpLoc = gl.GetUniformLocation(shader.program, "u_mvp");
				modelLoc = gl.GetUniformLocation(shader.program, "u_model");
				camPosLoc = gl.GetUniformLocation(shader.program, "u_camPos");

				vao = gl.GenVertexArray();
				gl.BindVertexArray(vao);

				vbo_pos = gl.GenBuffer();
				gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo_pos);

				float[] data = BasicMeshes.CubePositions;
				gl.BufferData<float>(BufferTargetARB.ArrayBuffer, data, BufferUsageARB.StaticDraw);

				gl.EnableVertexAttribArray(0);
				gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 0, IntPtr.Zero.ToPointer());
			}
		}
	}
}
