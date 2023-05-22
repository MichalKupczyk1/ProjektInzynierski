using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class AMF
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
            var goodPixels = new bool[WindowSize];
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
                                goodPixels[index] = !CorruptedPixels[i + l, j + k];
                                index++;
                            }
                        }
                        var pixel = CalculateMean(tempPixels, goodPixels);
                        pixelClone[i, j] = pixel;
                        index = 0;
                    }
                }
            }
            return pixelClone;
        }

        private Pixel CalculateMean(Pixel[] tempPixels, bool[] goodPixels)
        {
            int r = 0, g = 0, b = 0;

            int amount = 0;
            for (int i = 0; i < 9; i++)
            {
                if (i == 4)
                    continue;
                if (goodPixels[i])
                {
                    r += tempPixels[i].R;
                    g += tempPixels[i].G;
                    b += tempPixels[i].B;
                    amount++;
                }
            }
            if (amount > 2)
                return new Pixel((byte)(r / amount), (byte)(g / amount), (byte)(b / amount));
            else
                return tempPixels[4];
        }
    }
}
