using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Structures.BitmapClasses
{
    public class BitmapInfo
    {
        public bool[,] CorruptedPixels { get; set; }
        public Pixel[,] Pixels { get; set; }
        public int WindowSize { get; set; } = 9;
        public long Width { get; set; }
        public long Height { get; set; }
        public double Threshold { get; set; }
    }
}
