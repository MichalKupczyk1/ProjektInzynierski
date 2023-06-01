using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public static class PixelUtils
    {
        public static double CalculateEuclideanDistance(Pixel left, Pixel right)
        {
            return Math.Sqrt(Math.Pow(right.R - left.R, 2) + Math.Pow(right.G - left.G, 2) + Math.Pow(right.B - left.B, 2));
        }

        public static Pixel CalculateVectorMedian(Pixel[] tempPixels)
        {
            var distances = CalculateDistances(tempPixels);
            var dictionary = CalculateSum(distances);
            var result = dictionary.Where(x => x.Value == dictionary.Min(y => y.Value)).FirstOrDefault();

            return tempPixels[Convert.ToInt32(result.Key)];
        }

        public static Dictionary<string, double> CalculateSum(double[,] distances)
        {
            var resultDictionary = new Dictionary<string, double>();

            for (int i = 0; i < 9; i++)
            {
                var sum = 0.0;
                for (int j = 0; j < 9; j++)
                    sum += distances[i, j];

                resultDictionary.Add(i.ToString(), sum);
            }
            return resultDictionary;
        }

        public static double[,] CalculateDistances(Pixel[] pixels)
        {
            var distances = new double[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var left = pixels[i];
                    var right = pixels[j];

                    distances[i, j] = CalculateEuclideanDistance(left, right);
                    distances[j, i] = distances[i, j];
                }
            }
            return distances;
        }
    }
}
