using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public class FAPG : NoiseDetection
    {
        public override bool[,] DetectNoise()
        {
            DetectedNoise = new bool[Height, Width];

            var tempPixels = new Pixel[WindowSize];
            var index = 0;

            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            tempPixels[index++] = Pixels[i + l, j + k];
                        }
                    }
                    DetectedNoise[i, j] = IsCorrupted(tempPixels);
                    index = 0;
                }
            }
            return DetectedNoise;
        }

        private bool IsCorrupted(Pixel[] pixels)
        {
            var amount = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (i == 4)
                    continue;

                var distance = CalculateDistance(pixels[4], pixels[i]);
                if (distance < (double)Threshold)
                    amount++;
            }
            return amount < 2;
        }

        private double CalculateDistance(Pixel left, Pixel right)
        {
            return Math.Sqrt(Math.Pow(right.R - left.R, 2) + Math.Pow(right.G - left.G, 2) + Math.Pow(right.B - left.B, 2));
        }
    }
}
