﻿global using vec3 = Silk.NET.Maths.Vector3D<float>;
global using vec4 = Silk.NET.Maths.Vector4D<float>;
global using mat4 = Silk.NET.Maths.Matrix4X4<float>;

using HackNotts;
using HackNotts.Graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Numerics;

class Program
{
	public static Program Instance;

	public IWindow window;
	public IInputContext input;
	public IKeyboard kb;
	public IMouse mouse;

	public Vector2 mousePos;
	public Vector2 lastMousePos;

	public GL gl;

	public vec3 pos = new vec3(0, 0, 5);
	public float yaw = 0;
	public float pitch = 0;

	public vec3 CamPos;

	public mat4 projMat;
	public mat4 viewMat;

	public float fovY = Util.Deg2Rad(90);
	public float nearPlaneDistance = 0.1f;
	public float farPlaneDistance = 100;

	public int FrameNumber = 0;
	public float GameTime = 0;

	Program()
	{
        var options = WindowOptions.Default;
        options.Title = "HackNotts";

        window = Window.Create(options);

        window.Load += Window_Load;
        window.Update += Window_Update;
        window.Render += Window_Render;
    }

	private void Window_Load()
	{
		window.VSync = true;

		input = window.CreateInput();
		kb = input.Keyboards[0];
		
		//mouse = input.Mice[0];
  //      mouse.Position = (Vector2)(window.Position);
		//lastMousePos = mouse.Position;

        gl = window.CreateOpenGL();

		Ball.InitResources();

		unsafe
		{
			byte* s = gl.GetString(GLEnum.Version);
			Console.WriteLine(Marshal.PtrToStringAnsi((nint)s));
		}
	}

	private void Window_Update(double dt)
	{
		//lastMousePos = mousePos;
		//mousePos = mouse.Position;
		//var dm = mousePos - lastMousePos;

		//      mouse.Position = (Vector2)(window.Position);

		//      float mouseSensitivity = 1;
		//yaw += dm.X * mouseSensitivity * (float)dt;
		//pitch += dm.Y * mouseSensitivity * (float)dt;

		float rotSpeed = Util.Deg2Rad(180);
		if (kb.IsKeyPressed(Key.Left))
		{
			yaw -= rotSpeed * (float)dt;
		}
        if (kb.IsKeyPressed(Key.Right))
        {
            yaw += rotSpeed * (float)dt;
        }
        if (kb.IsKeyPressed(Key.Up))
        {
            pitch -= rotSpeed * (float)dt;
        }
        if (kb.IsKeyPressed(Key.Down))
        {
            pitch += rotSpeed * (float)dt;
        }

        pitch = Math.Clamp(pitch, -MathF.PI / 2, MathF.PI / 2);

		if (kb.IsKeyPressed(Key.Escape))
		{
			window.Close();
		}

		vec3 forward = new vec3(MathF.Sin(yaw), 0, -MathF.Cos(yaw));
		vec3 right = new vec3(MathF.Cos(yaw), 0, MathF.Sin(yaw));
		float speed = 5;
		if (kb.IsKeyPressed(Key.W))
		{
			pos += forward * speed * (float)dt;
		}
        if (kb.IsKeyPressed(Key.S))
        {
            pos -= forward * speed * (float)dt;
        }
        if (kb.IsKeyPressed(Key.A))
        {
            pos -= right * speed * (float)dt;
        }
        if (kb.IsKeyPressed(Key.D))
        {
            pos += right * speed * (float)dt;
        }

        float aspectRatio = (float)window.Size.X / window.Size.Y;
		projMat = Matrix4X4.CreatePerspectiveFieldOfView(fovY, aspectRatio, nearPlaneDistance, farPlaneDistance);

		CamPos = pos;
		viewMat = Matrix4X4.CreateTranslation(-CamPos) * Matrix4X4.CreateRotationY(yaw) * Matrix4X4.CreateRotationX(pitch);
		//CamPos = new vec3(3, 3, 3);
		//vec3 target = vec3.Zero;
		//vec3 up = vec3.UnitY;
		//viewMat = Matrix4X4.CreateLookAt(CamPos, target, up);

		FrameNumber++;
		GameTime = FrameNumber / 60.0f;
	}
	
	private void Window_Render(double dt)
	{
		gl.ClearColor(Color.CornflowerBlue);
		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.CullFace);
		
		gl.Enable(EnableCap.Blend);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		Ball.Balls.Add(new Ball.BallData { Pos = new vec3(0, 0, 0) });
		Ball.Draw();
	}

	static void Main(string[] args)
	{
		Instance = new Program();
		Instance.window.Run();
    }
}