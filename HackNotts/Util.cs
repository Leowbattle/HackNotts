using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackNotts
{
    internal static class Util
    {
        public static float Deg2Rad(float deg)
        {
            return deg * MathF.PI / 180;
        }

        public static float Rad2Deg(float rad)
        {
            return rad * 180 / MathF.PI;
        }
    }
}
