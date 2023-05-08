using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseRemoval
{
    public abstract class NoiseRemoval : BitmapInfo
    {
        public abstract Pixel[,] RemoveNoise();
        protected void ChangePixel(Pixel pixel, int x, int y)
        {
            Pixels[x, y] = pixel;
        }
    }
}
