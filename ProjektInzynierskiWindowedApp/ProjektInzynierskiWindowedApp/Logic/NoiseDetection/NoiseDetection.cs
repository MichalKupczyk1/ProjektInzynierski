using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public abstract class NoiseDetection : BitmapInfo
    {
        public abstract bool[,] DetectNoise();
    }
}
