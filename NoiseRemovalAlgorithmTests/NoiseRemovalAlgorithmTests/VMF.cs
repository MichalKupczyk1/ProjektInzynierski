using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class VMF
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public int WindowSize { get; set; }
        public int Threshold { get; set; }
        public Pixel[,] Pixels { get; set; }
        public bool[,] CorruptedPixels { get; set; }

        public Pixel[,] RemoveNoise()
        {
            var index = 0;
            var tempPixels = new Pixel[WindowSize];
            var pixelClone = Pixels.Clone() as Pixel[,];

            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    if (CorruptedPixels[i, j])
                    {
                        for (int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                tempPixels[index] = Pixels[i + l, j + k];
                                index++;
                            }
                        }
                        pixelClone[i, j] = VMFReplacement(tempPixels);
                        index = 0;
                    }
                }
            }
            return pixelClone;
        }

        private Pixel VMFReplacement(Pixel[] tempPixels)
        {
            var distances = CalculateDistances(tempPixels);
            var dictionary = CalculateSum(distances);
            var result = dictionary.Where(x => x.Value == dictionary.Min(y => y.Value)).FirstOrDefault();

            return tempPixels[Convert.ToInt32(result.Key)];
        }

        private Dictionary<string, double> CalculateSum(double[,] distances)
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

        private double[,] CalculateDistances(Pixel[] pixels)
        {
            var distances = new double[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var left = pixels[i];
                    var right = pixels[j];

                    distances[i, j] = Math.Sqrt(Math.Pow(right.R - left.R, 2) + Math.Pow(right.G - left.G, 2) + Math.Pow(right.B - left.B, 2));
                    distances[j, i] = distances[i, j];
                }
            }
            return distances;
        }
    }
}
