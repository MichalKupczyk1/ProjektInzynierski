using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseRemoval
{
    public class SumRemoval : NoiseRemoval
    {
        public override Pixel[,] RemoveNoise()
        {
            var tempPixels = new Pixel[WindowSize];
            var index = 0;
            var differenceArray = new double[WindowSize, WindowSize];

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
                    PixelManager.GetDifferenceTable(ref differenceArray, ref tempPixels);
                    //calculating sum
                    var sum = CalculateSum(differenceArray);
                    //returns true if pixel is corrupted
                    if (IsCorrupted(sum))
                        ChangePixel(tempPixels[(int)sum.Min()], i, j);

                    index = 0;
                }
            }
            return Pixels;
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
    }
}
