using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Linq;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public class FAST : NoiseDetection
    {
        private double M { get => 2.0; }

        public override bool[,] DetectNoise()
        {
            var detectedNoise = new bool[Height, Width];

            var trimmedDistancesResult = CalculateTrimmedDistances();
            var trimmedDistancesData = trimmedDistancesResult;
            var trimmedDistancesCalculation = trimmedDistancesResult;

            var minImpulsiveness = new double[WindowSize];
            var counter = 0;

            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            minImpulsiveness[counter++] = trimmedDistancesData[i + l, j + k];
                        }
                    }
                    var substraction = FindMinTrimmedDistanceWithinWindow(minImpulsiveness);
                    trimmedDistancesCalculation[i, j] = (short)(trimmedDistancesData[i, j] - substraction);

                    detectedNoise[i, j] = trimmedDistancesCalculation[i, j] / M > Threshold;

                    counter = 0;
                }
            }
            return detectedNoise;
        }

        private double FindMinTrimmedDistanceWithinWindow(double[] array)
        {
            var temp = array[0];
            for (int i = 1; i < WindowSize; i++)
            {
                if (array[i] < temp)
                    temp = array[i];
            }
            return temp;
        }

        private double[,] CalculateTrimmedDistances()
        {
            var index = 0;
            var tempPixels = new Pixel[WindowSize];

            var trimmedDistances = new double[Height, Width];

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
                    trimmedDistances[i, j] = CalculateDistance(tempPixels); //suma dwoch dystansów najmniejszych z pominieciem dystansu do samego siebie
                    index = 0;
                }
            }
            return trimmedDistances;
        }

        private double CalculateDistance(Pixel[] tempPixels)
        {
            var firstMin = double.MaxValue;
            var secondMin = double.MaxValue;
            var middlePixel = tempPixels[4];

            for (int i = 0; i < 9; i++)
            {
                if (i == 4) //pominięcie środkowego
                    continue;
                var distance = PixelUtils.CalculateEuclideanDistance(middlePixel, tempPixels[i]);

                if (distance < firstMin)
                {
                    secondMin = firstMin;
                    firstMin = distance;
                    continue;
                }
                if (distance < secondMin)
                {
                    secondMin = distance;
                }
            }
            return firstMin + secondMin;
        }
    }
}
