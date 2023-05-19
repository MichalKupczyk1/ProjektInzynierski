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
                        var pixel = CalculateMedian(tempPixels, goodPixels);
                        pixelClone[i, j] = pixel;
                        index = 0;
                    }
                }
            }
            return pixelClone;
        }

        private Pixel CalculateMedian(Pixel[] tempPixels, bool[] goodPixels)
        {
            var rs = new short[goodPixels.Length];
            var gs = new short[goodPixels.Length];
            var bs = new short[goodPixels.Length];
            var counter = 0;

            for (int i = 0; i < 9; i++)
            {
                if (i == 4 || !goodPixels[i])
                    continue;

                rs[counter] = tempPixels[i].R;
                gs[counter] = tempPixels[i].G;
                bs[counter] = tempPixels[i].B;
                counter++;
            }

            if (counter > 0)
            {
                var R = counter % 2 == 0 ? rs[counter / 2 + 1] : ((rs[counter / 2] + rs[counter / 2 + 1]) / 2);
                var G = counter % 2 == 0 ? gs[counter / 2 + 1] : ((gs[counter / 2] + gs[counter / 2 + 1]) / 2);
                var B = counter % 2 == 0 ? bs[counter / 2 + 1] : ((bs[counter / 2] + bs[counter / 2 + 1]) / 2);
                return new Pixel((byte)R, (byte)G, (byte)B);
            }
            return tempPixels[4];
        }
    }
}
