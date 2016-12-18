/* Copyright 2016 by
 * Christopher Aicher
 * aicher83128@gmail.com
 * */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Heat
{
    public static class HeatLib
    {
        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static List<Color> GetGradients(Color start, Color end, int steps)
        {
            if (steps <= 1 | steps > 100) { throw new NotSupportedException(); }

            List<Color> l = new List<Color>();

            for (int i = 0; i < steps; i++)
            {
                l.Add(Color.FromArgb(
                    start.A + (int)(((end.A - start.A) / (steps - 1.0)) * i),
                    start.R + (int)(((end.R - start.R) / (steps - 1.0)) * i),
                    start.G + (int)(((end.G - start.G) / (steps - 1.0)) * i),
                    start.B + (int)(((end.B - start.B) / (steps - 1.0)) * i))
                );
            }

            return l;
        }

        public static List<Color> GetGradients(List<Color> colors, int steps)
        {
            if (steps <= 2) { throw new NotSupportedException(); }

            List<Color> l = new List<Color>();

            for (int i=0; i < colors.Count -1; i++)
            {
                foreach(Color c in GetGradients(colors[i], colors[i+1], steps)) { l.Add(c); }

                l.RemoveAt(l.Count-1);
            }

            return l;
        }

        public static IEnumerable<Color> GetColorsBetween(Color start, Color end, int count, bool include_start = false, bool include_end = false)
        {
            //if (count <= 0 | count > 100) { throw new NotSupportedException(); }

            if (include_start) { yield return start; }

            for (int i = 0; i < count; i++)
            {
                double lambda = (1.0 / (count + 1)) * (i + 1);

                yield return ColorInterpolator.InterpolateBetween(start, end, lambda);
            }

            if (include_end) { yield return end; }
        }

        private class ColorInterpolator
        {
            delegate byte comp(Color color);

            static comp r = color => color.R;
            static comp g = color => color.G;
            static comp b = color => color.B;

            public static Color InterpolateBetween(Color col1, Color col2, double lambda)
            {
                if (lambda < 0 || lambda > 1) {throw new ArgumentOutOfRangeException("lambda"); }

                Color color = Color.FromArgb(
                    InterComp(col1, col2, lambda, r),
                    InterComp(col1, col2, lambda, g),
                    InterComp(col1, col2, lambda, b)
                );

                return color;
            }

            static byte InterComp(Color a, Color b, double lambda, comp co)
            {
                return (byte)(co(a) + (co(b)
                            - co(a)) * lambda);
            }

        }
    }
}
