using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public abstract class NoiseDetectionClass
    {
        public Pixel[,] Pixels { get; set; }
        public bool[,] DetectedNoise { get; set; }

        public int WindowSize { get; set; } = 9;
        public int Width { get; set; }
        public int Height { get; set; }
        public int Threshold { get; set; }

        public abstract bool[,] DetectNoise();
    }
}
