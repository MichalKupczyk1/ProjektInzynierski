using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseRemoval
{
    public class VMF : NoiseRemoval
    {
        public override Pixel[,] RemoveNoise()
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
            return PixelUtils.CalculateVectorMedian(tempPixels);
        }
    }
}
