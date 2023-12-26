using System;
using System.Collections.Generic;
using System.Text;

namespace SocialTrender
{
    public class Utils
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float InverseLerp(float value, float a, float b)
        {
            return (value - a) / (b - a);
        }

        public static float Remap(float value, float a1, float b1, float a2, float b2)
        {
            return Lerp(a2, b2, InverseLerp(value, a1, b1));
        }

        public static float Remap01(float value, float a, float b)
        {
            return Lerp(0.0f, 1.0f, InverseLerp(value, a, b));
        }

        public static float Percent(int percentage, float a, float b)
        {
            return Lerp(0.0f, 1.0f, InverseLerp(percentage * 0.01f, a, b));
        }
    }
}
