using HackNotts.Graphics;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackNotts.Simulation
{
    internal class Simulation
    {
        public class Ball
        {
            public vec3 Pos;
            public vec3 Velocity;
        }

        public List<Ball> Balls = new List<Ball>();

        public vec3 Gravity = new vec3(0, -9.81f, 0);

        public void Update(float dt)
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                Ball b = Balls[i];
                b.Velocity += Gravity * dt;
                b.Pos += b.Velocity * dt;

                if (b.Pos.Y < 0)
                {
                    b.Velocity.Y *= -1;
                    b.Pos.Y = 0;
                }
            }

            for (int i = 0; i < Balls.Count; i++)
            {
                Ball balli = Balls[i];
                for (int j = 0; j < Balls.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Ball ballj = Balls[j];

                    if (Vector3D.DistanceSquared(balli.Pos, ballj.Pos) < 4)
                    {
                        var collisionPoint = (balli.Pos + ballj.Pos) / 2;
                        float penetrationDepth = Vector3D.Distance(balli.Pos, ballj.Pos) - 2;

                        var collisionNormali = Vector3D.Normalize(collisionPoint - balli.Pos);
                        balli.Pos += collisionNormali * penetrationDepth;
                        balli.Velocity = Vector3D.Reflect(balli.Velocity, collisionNormali);

                        var collisionNormalj = Vector3D.Normalize(collisionPoint - ballj.Pos);
                        ballj.Pos += collisionNormalj * penetrationDepth;
                        ballj.Velocity = Vector3D.Reflect(ballj.Velocity, collisionNormalj);
                    }
                }
            }
        }
    }
}
