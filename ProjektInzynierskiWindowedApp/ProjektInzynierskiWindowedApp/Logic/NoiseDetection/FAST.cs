using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Linq;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public class FAST : NoiseDetection
    {
        public override bool[,] DetectNoise()
        {
            var impulsivenessResults = CalculateImpulsiveness();
            var impulsivenessData = impulsivenessResults.impulsivenessData;
            var impulsivenessCalculation = impulsivenessResults.impulsivenessCalculation;


            var minImpulsiveness = new short[WindowSize];
            var substraction = 0;
            var counter = 0;

            for (int i = 1; i < Height; i++)
            {
                for (int j = 1; j < Width; j++)
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

                    DetectedNoise[i - 1, j - 1] = impulsivenessCalculation[i, j] < Threshold;

                    counter = 0;
                }
            }
            return DetectedNoise;
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

        private (short[,] impulsivenessData, short[,] impulsivenessCalculation) CalculateImpulsiveness()
        {
            var index = 0;
            var tempPixels = new Pixel[WindowSize];

            var impulsivenessCalculation = new short[Height + 2, Width + 2];
            var impulsivenessData = new short[Height + 2, Width + 2];

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
                    var difference = CalculateDistance(tempPixels);
                    impulsivenessData[i, j] = difference.Max();
                    impulsivenessCalculation[i, j] = impulsivenessData[i, j];
                    index = 0;
                }
            }
            return (impulsivenessData, impulsivenessCalculation);
        }

        private short[] CalculateDistance(Pixel[] tempPixels)
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

                difference[i] = distances.Min();
            }
            difference[4] = 0;
            return difference;
        }
    }
}
