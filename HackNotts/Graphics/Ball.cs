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

		static int mvpsLoc;
		static int modelsLoc;
		static int viewLoc;
		static int camPosLoc;
		static int posLoc;
		static int projLoc;
		static int numLoc;

		public class BallData
		{
			public vec3 Pos;
		}

		public static List<BallData> Balls = new List<BallData>();

		public static void Draw()
		{
			int maxBalls = 100;

			mat4[] models = new mat4[maxBalls];
			mat4[] mvps = new mat4[maxBalls];
			vec3[] poss = new vec3[maxBalls];

			for (int i = 0; i < Balls.Count; i++)
			{
				var ball = Balls[i];

				mat4 modelMat = Matrix4X4.CreateTranslation(ball.Pos);
				mat4 mvp = modelMat * Program.Instance.viewMat * Program.Instance.projMat;

				models[i] = modelMat;
				mvps[i] = mvp;
				poss[i] = ball.Pos;
			}

			gl.BindVertexArray(vao);

			gl.UseProgram(shader.program);

			unsafe
			{
				fixed (mat4* modelsp = models)
				fixed (mat4* mvpsp = mvps)
				fixed (vec3* posp = poss)
				{
					gl.UniformMatrix4(mvpsLoc, (uint)Balls.Count, false, (float*)mvpsp);
					gl.UniformMatrix4(modelsLoc, (uint)Balls.Count, false, (float*)modelsp);
					gl.Uniform3(posLoc, (uint)Balls.Count, (float*)posp);
				}

				vec3 camPos = Program.Instance.CamPos;
				gl.Uniform3(camPosLoc, camPos.X, camPos.Y, camPos.Z);

				mat4 viewMat = Program.Instance.viewMat;
				gl.UniformMatrix4(viewLoc, 1, false, (float*)&viewMat);

                mat4 projMat = Program.Instance.projMat;
                gl.UniformMatrix4(projLoc, 1, false, (float*)&projMat);
            }
			gl.Uniform1(numLoc, Balls.Count);

			//gl.DrawArrays(GLEnum.Triangles, 0, 36);
			gl.DrawArraysInstanced(GLEnum.Triangles, 0, 36, (uint)Balls.Count);
		}

		public static void InitResources()
		{
			unsafe
			{
				shader = Shader.FromFiles("Assets/Shaders/ball.vert", "Assets/Shaders/ball.frag");
				mvpsLoc = gl.GetUniformLocation(shader.program, "u_mvps");
				modelsLoc = gl.GetUniformLocation(shader.program, "u_models");
				camPosLoc = gl.GetUniformLocation(shader.program, "u_camPos");
				viewLoc = gl.GetUniformLocation(shader.program, "u_view");
				posLoc = gl.GetUniformLocation(shader.program, "u_pos");
				projLoc = gl.GetUniformLocation(shader.program, "u_proj");
				numLoc = gl.GetUniformLocation(shader.program, "u_num");

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
