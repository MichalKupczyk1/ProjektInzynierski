using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class FAST
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public int WindowSize { get; set; }
        public int Threshold { get; set; }
        public Pixel[,] Pixels { get; set; }

        public bool[,] DetectNoise()
        {
            var detectedNoise = new bool[Height, Width];

            var impulsivenessResults = CalculateImpulsiveness();
            var impulsivenessData = impulsivenessResults;
            var impulsivenessCalculation = impulsivenessResults;

            var minImpulsiveness = new short[WindowSize];
            var substraction = 0;
            var counter = 0;

            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            minImpulsiveness[counter++] = impulsivenessData[i + l, j + k];
                        }
                    }
                    substraction = FindImpulsiveness(minImpulsiveness);
                    impulsivenessCalculation[i, j] = (short)(impulsivenessData[i, j] - substraction);

                    detectedNoise[i, j] = impulsivenessCalculation[i, j] > Threshold;

                    counter = 0;
                }
            }
            return detectedNoise;
        }

        private short FindImpulsiveness(short[] array)
        {
            short temp = array[0];
            for (int i = 1; i < WindowSize; i++)
            {
                if (array[i] < temp)
                    temp = array[i];
            }
            return temp;
        }

        private short[,] CalculateImpulsiveness()
        {
            var index = 0;
            var tempPixels = new Pixel[WindowSize];

            var impulsivenessData = new short[Height, Width];

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
                    var difference = CalculateDistance(tempPixels, i, j);
                    impulsivenessData[i, j] = (short)FindMin(difference); //suma dwoch najmniejszych z pominieciem zera
                    index = 0;
                }
            }
            return impulsivenessData;
        }

        private int FindMin(short[] difference)
        {
            var firstMin = short.MaxValue;
            var secondMin = short.MaxValue;

            for (int i = 0; i < difference.Length; i++)
            {
                if (i == 4)
                    continue;
                if (difference[i] < firstMin)
                {
                    secondMin = firstMin;
                    firstMin = difference[i];
                    continue;
                }
                if (difference[i] < secondMin)
                {
                    secondMin = difference[i];
                }

            }
            return firstMin + secondMin;
        }

        private short[] CalculateDistance(Pixel[] tempPixels, int ii, int jj)
        {
            var distances = new short[3];
            var difference = new short[WindowSize];

            for (int i = 0; i < 9; i++)
            {
                if (i == 4)
                    continue;
                distances[0] = (short)Math.Abs(tempPixels[4].R - tempPixels[i].R);
                distances[1] = (short)Math.Abs(tempPixels[4].G - tempPixels[i].G);
                distances[2] = (short)Math.Abs(tempPixels[4].B - tempPixels[i].B);

                difference[i] = distances.Max();
            }
            difference[4] = 0;
            return difference;
        }
    }
}
