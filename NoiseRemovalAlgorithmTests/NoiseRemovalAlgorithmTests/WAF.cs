using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class WAF
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public int WindowSize { get; set; }
        public int Threshold { get; set; }
        public Pixel[,] Pixels { get; set; }
        public bool[,] CorruptedPixels { get; set; }
        private short[,] NeighborMap { get; set; }

        public Pixel[,] RemoveNoise()
        {
            CalculateAmountOfClosePixels();

            var tempPixels = new Pixel[WindowSize];
            var tempNeighborMap = new short[WindowSize];
            var tempCorruptedPixels = new bool[WindowSize];
            var index = 0;
            var pixels = Pixels.Clone() as Pixel[,];

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
                                tempNeighborMap[index] = NeighborMap[i + l, j + k];
                                tempCorruptedPixels[index++] = CorruptedPixels[i + l, j + k];
                            }
                        }
                        pixels[i, j] = CalculateWAF(tempPixels, tempNeighborMap, tempCorruptedPixels);
                    }
                    index = 0;
                }
            }
            return pixels;
        }

        private Pixel CalculateWAF(Pixel[] pixels, short[] neighborMap, bool[] corruptedPixels)
        {
            int R = 0, G = 0, B = 0;
            int amount = 0;

            for (int i = 0; i < neighborMap.Length; i++)
            {
                if (i == 4 || corruptedPixels[i])
                    continue;
                amount += neighborMap[i];
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                if (i == 4 || corruptedPixels[i])
                    continue;

                R += pixels[i].R * neighborMap[i];
                G += pixels[i].G * neighborMap[i];
                B += pixels[i].B * neighborMap[i];
            }
            if (amount > 0)
                return new Pixel((byte)(R / amount), (byte)(G / amount), (byte)(B / amount));
            return pixels[4];
        }

        private void CalculateAmountOfClosePixels()
        {
            NeighborMap = new short[Height, Width];
            var tempPixels = new bool[WindowSize];
            var index = 0;
            short counter = 0;

            for (int i = 0; i < Height; i++)
            {
                NeighborMap[i, 0] = 0;
                NeighborMap[i, Width - 1] = 0;
            }
            for (int i = 0; i < Width; i++)
            {
                NeighborMap[0, i] = 0;
                NeighborMap[Height - 1, i] = 0;
            }

            for (int i = 1; i < Height - 1; i++)
            {
                for (int j = 1; j < Width - 1; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        for (int l = -1; l < 2; l++)
                        {
                            tempPixels[index++] = CorruptedPixels[i + l, j + k];
                        }
                    }

                    foreach (var pixel in tempPixels)
                    {
                        if (!pixel)
                            counter++;
                    }

                    NeighborMap[i, j] = counter;
                    counter = 0;
                    index = 0;
                }
            }
        }
    }
}
