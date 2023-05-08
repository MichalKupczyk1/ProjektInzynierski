using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Logic.Utils
{
    public static class PixelManager
    {
        public static double CalculateDifference(Pixel left, Pixel right)
        {
            double R = left.R - right.R;
            double G = left.G - right.G;
            double B = left.B - right.B;

            return Math.Sqrt(R * R + G * G + B * B);
        }

        public static void GetDifferenceTable(ref double[,] difference, ref Pixel[] pixels)
        {
            for (int k = 0; k < 9; k++)
            {
                for (int l = k; l < 9; l++)
                {
                    difference[k, l] = CalculateDifference(pixels[k], pixels[l]);
                    difference[l, k] = difference[k, l];
                }
            }
        }
    }
}
