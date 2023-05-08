using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public class FAPG : NoiseDetectionClass
    {
        public override bool[,] DetectNoise()
        {
            var tempPixels = new Pixel[WindowSize];
            var difference = new double[WindowSize, WindowSize];
            var index = 0;

            for (int i = 1; i < Height + 1; i++)
            {
                for (int j = 1; j < Width + 1; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            tempPixels[index++] = Pixels[i + l, j + k];
                        }
                    }
                    //passing differenceArray through reference to save results over there
                    GetDifferenceTable(ref difference, ref tempPixels);
                    //calculating sum
                    var sum = CalculateSum(difference);
                    //returns true if pixel is corrupted, substracting one from indexes because bool array wasn't extended
                    DetectedNoise[i - 1, j - 1] = IsCorrupted(sum);

                    index = 0;
                }
            }
            return DetectedNoise;
        }

        private void GetDifferenceTable(ref double[,] difference, ref Pixel[] pixels)
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

        private double CalculateDifference(Pixel left, Pixel right)
        {
            double R = left.R - right.R;
            double G = left.G - right.G;
            double B = left.B - right.B;

            return Math.Sqrt(R * R + G * G + B * B);
        }

        private double[] CalculateSum(double[,] difference)
        {
            var sum = new double[WindowSize];
            int it = 0;
            for (int i = 0; i < WindowSize; i++)
                sum[i] = 0;

            for (int i = 0; i < WindowSize; i++)
            {
                for (int j = 0; j < WindowSize; j++)
                    sum[it] += difference[i, j];
                it++;
            }
            return sum;
        }
        private bool IsCorrupted(double[] sum)
        {
            int goodPixels = 0;

            for (int i = 0; i < WindowSize; i++)
            {
                if (i == 4)
                    continue;
                if (sum[i] < Threshold)
                    goodPixels++;
            }
            return goodPixels < 3;
        }
    }
}
