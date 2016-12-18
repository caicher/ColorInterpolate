using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            /* ACHTUNG:
             * Bei >100 Steps funktioniert es nicht mehr, wieso???
             * Bei manchen (z.B. 96) geht es auch nicht ganz auf
             * */
            if (steps <= 1 | steps > 100) { throw new NotSupportedException(); }

            List<Color> l = new List<Color>();

            double stepA = ((end.A - start.A) / (double)(steps - 1));
            double stepR = ((end.R - start.R) / (double)(steps - 1));
            double stepG = ((end.G - start.G) / (double)(steps - 1));
            double stepB = ((end.B - start.B) / (double)(steps - 1));

            for (int i = 0; i < steps; i++)
            {
                l.Add(Color.FromArgb(
                    start.A + (int)(stepA * i),
                    start.R + (int)(stepR * i),
                    start.G + (int)(stepG * i),
                    start.B + (int)(stepB * i))
                );
            }

            return l;
        }



        // Zwischen jeder Farbe liegen die Steps
        public static List<Color> GetGradients(List<Color> colors, int steps)
        {
            // Wäre ja Blödsinn, dan würden nur 2 Farben zurück gegeben werden
            if (steps <= 2) { throw new NotSupportedException(); }
            // Bei 3 würde 1 neue Farbe kommen, die dazwischen ist

            List<Color> l = new List<Color>();

            // Anzahl Ergebnisse:
            // int ii = (steps - 1) * (colors.Count - 1);

            for (int i=0; i < colors.Count -1; i++)
            {
                foreach(Color c in GetGradients(colors[i], colors[i+1], steps))
                {
                    l.Add(c);
                }

                l.RemoveAt(l.Count-1);
            }

            return l;
        }

        public static IEnumerable<Color> GetColorsBetween(Color start, Color end, int count, bool include_start = false, bool include_end = false)
        {
            //if (count <= 0 | count > 100) { throw new NotSupportedException(); }

            /*
             * Wenn ich zwei Werte haben möchte, dann bräuchte ich ja den bei 33,33% und den bei 66,66%
             * Der Divisor ist also: (Anzahl der Werte + 1)
             * 1/(Anzahl der Werte + 1) * (iterator + 1) {wenn iterator_index==0);
             * */

            if (include_start) { yield return start; }

            for (int i = 0; i < count; i++)
            {
                double lambda = (1.0 / (count + 1)) * (i + 1);
                //Hä wieso geht 1 / ((count + 1) * (i + 1)) nicht? Kommt immer 0 raus??
                //lol Fehler gefunden, der Compiler "sieht" zwei int, wodurch die Dividion per int läuft
                //Also int zu double machen, indem man nicht 1 sondern 1.0 verwendet.

                yield return ColorInterpolator.InterpolateBetween(start, end,
                    lambda);
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
