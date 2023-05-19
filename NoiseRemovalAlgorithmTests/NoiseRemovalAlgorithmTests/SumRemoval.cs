using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class SumRemoval
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public int WindowSize { get; set; }
        public int Threshold { get; set; }
        public Pixel[,] Pixels { get; set; }
        public bool[,] CorruptedPixels { get; set; }

        public Pixel[,] RemoveNoise()
        {
            var tempPixels = new Pixel[WindowSize];
            var index = 0;
            var differenceArray = new double[WindowSize, WindowSize];

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
                    //passing differenceArray through reference to save results over there
                    GetDifferenceTable(ref differenceArray, ref tempPixels);
                    //calculating sum
                    var sum = CalculateSum(differenceArray);
                    //returns true if pixel is corrupted
                    if (IsCorrupted(sum))
                    {
                        Pixels[i, j] = tempPixels[ReturnIndexOfMin(sum)];
                        CorruptedPixels[i, j] = true;
                    }
                    else
                        CorruptedPixels[i, j] = false;

                    index = 0;
                }
            }
            return Pixels;
        }

        private int ReturnIndexOfMin(double[] sum)
        {
            var max = sum.Max();
            var index = -1;
            for (int i = 0; i < sum.Length; i++)
            {
                if (sum[i] < max)
                {
                    max = sum[i];
                    index = i;
                }
            }
            return index;
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
                {
                    sum[it] += difference[i, j];
                }
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
    }
}
