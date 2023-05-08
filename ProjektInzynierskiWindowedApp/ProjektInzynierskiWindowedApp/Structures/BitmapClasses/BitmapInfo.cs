using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Structures.BitmapClasses
{
    public class BitmapInfo
    {
        public bool[,] DetectedNoise { get; set; }
        public Pixel[,] Pixels { get; set; }
        public int WindowSize { get; set; } = 9;
        public long Width { get; set; }
        public long Height { get; set; }
        public int Threshold { get; set; }
    }
}
